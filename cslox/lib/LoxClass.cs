public class LoxClass(string name, Dictionary<string, LoxFunction> methods) : ILoxCallable, ILoxInstance
{
  private readonly Dictionary<string, LoxFunction> Methods = methods;
  public string Name => name;
  public LoxClass Class => this;
  public Dictionary<string, object?> Fields { get; } = [];

  public int Arity
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

  public object? Call(Interpreter interpreter, List<object?> arguments)
  {
    LoxInstance instance = new(this);
    LoxFunction? initializer = FindMethod("init");
    initializer?.Bind(instance).Call(interpreter, arguments);
    return instance;
  }

  public void Set(Token name, object? value)
  {
    throw new NotImplementedException();
  }

  public object? Get(Token name)
  {
    throw new NotImplementedException();
  }

  public override string ToString()
  {
    return $"<class {Name}>";
  }
}
