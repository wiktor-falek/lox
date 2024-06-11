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
    }
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
    // TODO: update current
  }

  private bool IsAtEnd()
  {
    return Current > Source.Length;
  }
}