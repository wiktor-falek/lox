public class LoxInstance(LoxClass @class)
{
  private readonly LoxClass @Class = @class;
  private readonly Dictionary<string, object?> fields = [];

  public object? Get(Token name)
  {
    if (fields.TryGetValue(name.Lexeme, out var value))
    {
      return value;
    }

    LoxFunction? method = @Class.FindMethod(name.Lexeme);
    if (method is not null) return method.Bind(this);

    throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
  }

  public void Set(Token name, object? value)
  {
    fields.Remove(name.Lexeme);
    fields.Add(name.Lexeme, value);
  }

  public override string ToString()
  {
    return $"<class {@Class.Name} instance>";
  }
}
