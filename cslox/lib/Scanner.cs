public class Scanner(string source)
{
  private string Source = source;
  public List<Token> Tokens = [];
  private int Start = 0;
  private int Current = 0;
  private int Line = 1;

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
    char? c = Advance();

    if (c == null) return;

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
        Lox.Error(Line, $"Unexpected character: {c}"); break;
    }
  }

  private void String()
  {
    // TODO
  }

  private char Peek()
  {
    if (IsAtEnd()) return '\0';
    return Source.ElementAt(Current);
  }

  private char? Advance()
  {
    try
    {
      char c = Source.ElementAt(Current);
      Current++;
      return c;
    }
    catch (ArgumentOutOfRangeException)
    {
      return null;
    }
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