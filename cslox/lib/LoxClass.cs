public class LoxClass(
  string name,
  LoxClass? superclass,
  Dictionary<string, LoxFunction> methods,
  Dictionary<string, LoxFunction> staticMethods
) : ILoxCallable, ILoxInstance
{
  private readonly Dictionary<string, LoxFunction> Methods = methods;
  private readonly Dictionary<string, LoxFunction> StaticMethods = staticMethods;
  public LoxClass? Superclass = superclass;
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
    if (Methods.TryGetValue(name, out var method))
    {
      return method;
    };

    LoxFunction? staticMethod = FindStaticMethod(name);

    if (staticMethod is not null)
    {
      return staticMethod;
    }

    if (Superclass is not null)
    {
      return Superclass.FindMethod(name);
    }

    return method;
  }

  public LoxFunction? FindStaticMethod(string name)
  {
    if (StaticMethods.TryGetValue(name, out var method))
    {
      return method;
    }

    return Superclass?.FindStaticMethod(name);
  }

  public object? Call(Interpreter interpreter, List<object?> arguments)
  {
    LoxInstance instance = new(this);
    LoxFunction? initializer = FindMethod("init");
    initializer?.Bind(instance).Call(interpreter, arguments);
    return instance;
  }

  public object? Get(Token name)
  {
    if (Fields.TryGetValue(name.Lexeme, out var value))
    {
      return value;
    }

    LoxFunction? method = FindStaticMethod(name.Lexeme);

    if (method is not null)
    {
      return method;
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
    return $"<class {Name}>";
  }
}
