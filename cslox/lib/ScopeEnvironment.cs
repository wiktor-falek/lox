public class ScopeEnvironment(ScopeEnvironment? enclosing = null)
{
  private readonly ScopeEnvironment? Enclosing = enclosing;
  private readonly List<object?> Values = [];

  public void Define(object? value)
  {
    Values.Add(value);
  }

  public object? GetAt(int distance, int slot)
  {
    return Ancestor(distance).Values[slot];
  }

  public void AssignAt(int distance, int slot, object? value)
  {
    Ancestor(distance).Values[slot] = value;
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
}
