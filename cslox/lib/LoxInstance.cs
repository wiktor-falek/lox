public interface ILoxInstance
{
  LoxClass Class { get; }
  Dictionary<string, object?> Fields { get; }
  object? Get(Token name);
  void Set(Token name, object? value);
}

public class LoxInstance(LoxClass @class) : ILoxInstance
{
  public LoxClass Class { get; } = @class;
  public Dictionary<string, object?> Fields { get; } = [];

  public object? Get(Token name)
  {
    if (Fields.TryGetValue(name.Lexeme, out var value))
    {
      return value;
    }

    LoxFunction? method = Class.FindMethod(name.Lexeme);

    if (method is not null)
    {
      return method.Bind(this);
      // if is a getter return method.Call()
    }

    throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
  }

  public void Set(Token name, object? value)
  {
    Fields.Remove(name.Lexeme);
    Fields.Add(name.Lexeme, value);
  }

  public override string ToString()
  {
    return $"<class {Class.Name} instance>";
  }
}
