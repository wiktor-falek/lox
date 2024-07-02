class RuntimeEnvironment
{
  private readonly Dictionary<string, object?> values = [];

  public object? Get(Token name)
  {
    if (values.TryGetValue(name.Lexeme, out object? value))
    {
      return value;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
  }

  public void Define(string name, object? value)
  {
    values[name] = value;
  }

  public void Assign(Token name, object? value)
  {
    if (values.ContainsKey(name.Lexeme))
    {
      values[name.Lexeme] = value;
    }
    else
    {
      throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
  }
}
