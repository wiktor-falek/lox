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
}
