public class Token
{
  public readonly TokenType Type;
  public readonly string Lexeme;
  public readonly object? Literal;
  public readonly int Line;

  public Token(TokenType type, string lexeme, object? literal, int line)
  {
    Type = type;
    Lexeme = lexeme;
    Literal = literal;
    Line = line;
  }

  override public string ToString()
  {
    return $"Token({Type}, \"{Lexeme}\", {Literal ?? "null"})";
  }
}