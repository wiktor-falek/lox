using static TokenType;

public class Scanner(string source)
{
  private readonly List<Token> Tokens = [];
  private readonly string Source = source;
  private int Start = 0;
  private int Current = 0;
  private int Line = 1;
  private readonly Dictionary<string, TokenType> Keywords = new()
  {
      {"and", AND},
      {"class", CLASS},
      {"else", ELSE},
      {"false", FALSE},
      {"for", FOR},
      {"fun", FUN},
      {"if", IF},
      {"nil", NIL},
      {"or", OR},
      {"print", PRINT},
      {"return", RETURN},
      {"super", SUPER},
      {"this", THIS},
      {"true", TRUE},
      {"var", VAR},
      {"while", WHILE},
  };

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      Start = Current;
      ScanToken();
    }

    Tokens.Add(new Token(EOF, "", null, Line));

    return Tokens;
  }

  private void ScanToken()
  {
    char c = Advance();

    switch (c)
    {
      case '(': AddToken(LEFT_PAREN); break;
      case ')': AddToken(RIGHT_PAREN); break;
      case '{': AddToken(LEFT_BRACE); break;
      case '}': AddToken(RIGHT_BRACE); break;
      case ',': AddToken(COMMA); break;
      case '.': AddToken(DOT); break;
      case '-': AddToken(MINUS); break;
      case '+': AddToken(PLUS); break;
      case ':': AddToken(COLON); break;
      case ';': AddToken(SEMICOLON); break;
      case '*': AddToken(STAR); break;
      case '?': AddToken(QUESTION_MARK); break;
      case '!':
        AddToken(Match('=') ? BANG_EQUAL : BANG); break;
      case '=':
        AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
      case '<':
        AddToken(Match('=') ? LESS_EQUAL : LESS); break;
      case '>':
        AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
      case '/':
        if (Match('/'))
        {
          while (Peek() != '\n' && !IsAtEnd()) Advance();
        }
        else
        {
          AddToken(SLASH);
        }
        break;
      case ' ':
      case '\r':
      case '\t':
        break;
      case '\n':
        Line++;
        break;
      case '"': String(); break;
      default:
        if (char.IsDigit(c))
        {
          Number();
        }
        else if (char.IsLetter(c))
        {
          Identifier();
        }
        else
        {
          Lox.Error(Line, $"Unexpected character: {c}"); break;
        }
        break;
    }
  }

  private void String()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\n') Line++;
      Advance();
    }

    if (IsAtEnd())
    {
      Lox.Error(Line, "Unterminated string.");
      return;
    }

    // Closing "
    Advance();

    string value = Source[(Start + 1)..(Current - 1)];
    AddToken(STRING, value);
  }

  private void Number()
  {
    while (char.IsDigit(Peek())) Advance();

    if (Peek() == '.' && char.IsDigit(PeekNext()))
    {
      // consume '.'
      Advance();

      while (char.IsDigit(Peek())) Advance();
    }

    float value = float.Parse(Source[Start..Current]);
    AddToken(NUMBER, value);
  }

  private void Identifier()
  {
    while (char.IsLetterOrDigit(Peek())) Advance();

    string text = Source[Start..Current];

    TokenType type = Keywords.TryGetValue(text, out var keyword)
    ? keyword
    : IDENTIFIER;

    AddToken(type);
  }

  private char Peek()
  {
    if (IsAtEnd()) return '\0';
    return Source.ElementAt(Current);
  }

  private char PeekNext()
  {
    if (Current + 1 >= Source.Length) return '\0';
    return Source.ElementAt(Current + 1);
  }

  private char Advance()
  {
    return Source.ElementAt(Current++);
  }

  private void AddToken(TokenType token)
  {
    AddToken(token, null);
  }

  private void AddToken(TokenType type, object? literal)
  {
    string text = Source[Start..Current];
    Token token = new(type, text, literal, Line);
    Tokens.Add(token);
  }

  private bool IsAtEnd()
  {
    return Current >= Source.Length;
  }

  private bool Match(char expected)
  {
    if (IsAtEnd()) return false;
    if (Source.ElementAt(Current) != expected) return false;

    Current++;
    return true;
  }
}