public class LoxClass(string name, Dictionary<string, LoxFunction> methods) : LoxCallable
{
  public override string Name => name;
  public override int Arity => 0;
  public readonly Dictionary<string, LoxFunction> Methods = methods;

  public LoxFunction? FindMethod(string name)
  {
    Methods.TryGetValue(name, out var method);
    return method;
  }

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    LoxInstance instance = new(this);
    return instance;
  }

  public override string ToString()
  {
    return $"<class {Name}>";
  }
}
