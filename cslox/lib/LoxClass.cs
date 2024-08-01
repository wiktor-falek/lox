public class LoxClass(string name, Dictionary<string, LoxFunction> methods) : LoxCallable
{
  public override string Name => name;
  public readonly Dictionary<string, LoxFunction> Methods = methods;

  public override int Arity
  {
    get
    {
      LoxFunction? initializer = FindMethod("init");
      return initializer?.Arity ?? 0;
    } 
  }

  public LoxFunction? FindMethod(string name)
  {
    Methods.TryGetValue(name, out var method);
    return method;
  }

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    LoxInstance instance = new(this);
    LoxFunction? initializer = FindMethod("init");
    initializer?.Bind(instance).Call(interpreter, arguments);
    return instance;
  }

  public override string ToString()
  {
    return $"<class {Name}>";
  }
}
