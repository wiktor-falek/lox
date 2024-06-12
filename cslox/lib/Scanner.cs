public class Scanner(string source)
{
  private readonly List<Token> Tokens = [];
  private readonly string Source = source;
  private int Start = 0;
  private int Current = 0;
  private int Line = 1;
  private readonly Dictionary<string, TokenType> Keywords = new()
  {
      {"and", TokenType.AND},
      {"class", TokenType.CLASS},
      {"else", TokenType.ELSE},
      {"false", TokenType.FALSE},
      {"for", TokenType.FOR},
      {"fun", TokenType.FUN},
      {"if", TokenType.IF},
      {"nil", TokenType.NIL},
      {"or", TokenType.OR},
      {"print", TokenType.PRINT},
      {"return", TokenType.RETURN},
      {"super", TokenType.SUPER},
      {"this", TokenType.THIS},
      {"true", TokenType.TRUE},
      {"var", TokenType.VAR},
      {"while", TokenType.WHILE},
  };

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      Start = Current;
      ScanToken();
    }

    Tokens.Add(new Token(TokenType.EOF, "", null, Line));

    return Tokens;
  }

  private void ScanToken()
  {
    char c = Advance();

    switch (c)
    {
      case '(': AddToken(TokenType.LEFT_PAREN); break;
      case ')': AddToken(TokenType.RIGHT_PAREN); break;
      case '{': AddToken(TokenType.LEFT_BRACE); break;
      case '}': AddToken(TokenType.RIGHT_BRACE); break;
      case ',': AddToken(TokenType.COMMA); break;
      case '.': AddToken(TokenType.DOT); break;
      case '-': AddToken(TokenType.MINUS); break;
      case '+': AddToken(TokenType.PLUS); break;
      case ';': AddToken(TokenType.SEMICOLON); break;
      case '*': AddToken(TokenType.STAR); break;
      case '!':
        AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
      case '=':
        AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
      case '<':
        AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
      case '>':
        AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
      case '/':
        if (Match('/'))
        {
          while (Peek() != '\n' && !IsAtEnd()) Advance();
        }
        else
        {
          AddToken(TokenType.SLASH);
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
        else if (char.IsLetter(c)) // or _
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
    AddToken(TokenType.STRING, value);
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
    AddToken(TokenType.NUMBER, value);
  }

  private void Identifier()
  {
    while (char.IsLetterOrDigit(Peek())) Advance();

    string text = Source[Start..Current];

    TokenType type = Keywords.TryGetValue(text, out var keyword)
    ? keyword
    : TokenType.IDENTIFIER;

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