class ScopeEnvironment(ScopeEnvironment? enclosing = null)
{
  public readonly ScopeEnvironment? Enclosing = enclosing;
  private readonly Dictionary<string, object?> Values = [];

  public object? Get(Token name)
  {
    if (Values.TryGetValue(name.Lexeme, out object? value))
    {
      return value;
    }

    Enclosing?.Get(name);

    throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
  }

  public void Define(string name, object? value)
  {
    Values[name] = value;
  }

  public void Assign(Token name, object? value)
  {
    if (Values.ContainsKey(name.Lexeme))
    {
      Values[name.Lexeme] = value;
      return;
    }

    if (Enclosing is not null)
    {
      Enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
  }
}
