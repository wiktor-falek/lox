public interface ILoxCallable
{
  string Name { get; }
  int Arity { get; }
  object? Call(Interpreter interpreter, List<object?> arguments);
  string ToString();
}

public class LoxFunction(
  FunctionStmt declaration,
  ScopeEnvironment closure,
  bool isInitializer = false,
  bool isGetter = false
  ) : ILoxCallable
{
  public string Name => Declaration.Name.Lexeme;
  public int Arity => Declaration.Parameters.Count;
  private readonly FunctionStmt Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;
  private readonly bool IsInitializer = isInitializer;
  public readonly bool IsGetter = isGetter;

  public object? Call(Interpreter interpreter, List<object?> arguments)
  {
    ScopeEnvironment environment = new(Closure);

    for (int i = 0; i < Declaration.Parameters.Count; i++)
    {
      environment.Define(arguments[i]);
    }

    try
    {
      interpreter.ExecuteBlock(Declaration.Body, environment);
    }
    catch (Return returnValue)
    {
      if (IsInitializer) return Closure.GetAt(0, 0); // return 'this' in class initializer early return
      return returnValue.Value;
    }

    if (IsInitializer)
    {
      // return 'this' in class initializer
      return Closure.GetAt(0, 0);
    }

    return null;
  }

  public LoxFunction Bind(LoxInstance instance)
  {
    ScopeEnvironment environment = new(Closure);
    environment.Define(instance);
    return new LoxFunction(Declaration, environment, IsInitializer, IsGetter);
  }

  public override string ToString()
  {
    return $"<function {Name}>";
  }
}

public class LoxLambdaFunction(LambdaExpr declaration, ScopeEnvironment closure) : ILoxCallable
{
  public string Name => "(anonymous)";
  public int Arity => Declaration.Parameters.Count;
  private readonly LambdaExpr Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;

  public object? Call(Interpreter interpreter, List<object?> arguments)
  {
    ScopeEnvironment environment = new(Closure);

    for (int i = 0; i < Declaration.Parameters.Count; i++)
    {
      environment.Define(arguments[i]);
    }

    try
    {
      interpreter.ExecuteBlock(Declaration.Body, environment);
    }
    catch (Return returnValue)
    {
      return returnValue.Value;
    }

    return null;
  }

  public override string ToString()
  {
    return $"<function {Name}>";
  }
}
