public class ScopeEnvironment(ScopeEnvironment? enclosing = null)
{
  public readonly ScopeEnvironment? Enclosing = enclosing;
  private readonly Dictionary<string, object?> Values = [];

  public object? Get(Token name)
  {
    if (Values.TryGetValue(name.Lexeme, out object? value))
    {
      return value;
    }

    if (Enclosing is not null)
    {
      return Enclosing.Get(name);
    }

    throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
  }

  private ScopeEnvironment Ancestor(int distance)
  {
    ScopeEnvironment environment = this;
    for (int i = 0; i < distance; i++)
    {
      environment = environment.Enclosing!;
    }

    return environment;
  }

  public object? GetAt(int distance, string name)
  {
    return Ancestor(distance).Values[name];
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

  public void AssignAt(int distance, Token name, object? value)
  {
    ScopeEnvironment ancestor = Ancestor(distance);
    ancestor.Values.Remove(name.Lexeme);
    ancestor.Values.Add(name.Lexeme, value);
  }
}
