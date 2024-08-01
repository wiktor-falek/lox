public abstract class LoxCallable
{
  public abstract string Name { get; }
  public abstract int Arity { get; }
  public abstract object? Call(Interpreter interpreter, List<object?> arguments);

  public override string ToString()
  {
    return $"<function {Name}>";
  }
}

public class LoxFunction(FunctionStmt declaration, ScopeEnvironment closure, bool isInitializer) : LoxCallable
{
  public override string Name => Declaration.Name.Lexeme;
  public override int Arity => Declaration.Parameters.Count;
  private readonly FunctionStmt Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;
  private readonly bool IsInitializer = isInitializer;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
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
      if (IsInitializer) return Closure.GetAt(0, 0); // return 'this' in early return
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
    return new LoxFunction(Declaration, environment, IsInitializer);
  }
}

public class LoxLambdaFunction(LambdaExpr declaration, ScopeEnvironment closure) : LoxCallable
{
  public override string Name => "(anonymous)";
  public override int Arity => Declaration.Parameters.Count;
  private readonly LambdaExpr Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
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
}
