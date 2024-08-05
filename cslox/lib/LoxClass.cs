public class LoxClass(
  string name,
  Dictionary<string,
  LoxFunction> methods,
  Dictionary<string, LoxFunction> staticMethods
) : ILoxCallable, ILoxInstance
{
  private readonly Dictionary<string, LoxFunction> Methods = methods;
  private readonly Dictionary<string, LoxFunction> StaticMethods = staticMethods;
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

    if (method is null)
    {
      LoxFunction? staticMethod = FindStaticMethod(name);
      return staticMethod;
    }

    return method;
  }

  public LoxFunction? FindStaticMethod(string name)
  {
    StaticMethods.TryGetValue(name, out var method);
    return method;
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
