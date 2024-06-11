public class Scanner(string source)
{
  private string Source = source;
  private List<Token> Tokens = [];
  private int Start = 0;
  private int Current = 0;
  private int Line = 1;

  public List<Token> ScanTokens()
  {
    List<Token> arr = [];
    return arr;
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
      // TODO: wtf is carriage return
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
    string text = Source.Substring(Start, Current);
    Token token = new(type, text, literal, Line);
    _ = Tokens.Append(token);
  }

  private bool IsAtEnd()
  {
    return Current > Source.Length;
  }

  private bool Match(char expected)
  {
    if (IsAtEnd()) return false;
    if (Source.ElementAt(Current) != expected) return false;

    Current++;
    return true;
  }
}