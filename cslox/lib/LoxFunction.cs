public abstract class LoxCallable
{
  protected abstract string Name { get; }
  public abstract int Arity { get; }
  public abstract object? Call(Interpreter interpreter, List<object?> arguments);

  public override string ToString()
  {
    return $"<function {Name}>";
  }
}

public class LoxFunction(FunctionStmt declaration, ScopeEnvironment closure) : LoxCallable
{
  public override int Arity => Declaration.Parameters.Count;
  protected override string Name => Declaration.Name.Lexeme;
  private readonly FunctionStmt Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    ScopeEnvironment environment = new(Closure);

    for (int i = 0; i < Declaration.Parameters.Count; i++)
    {
      environment.Define(Declaration.Parameters[i].Lexeme, arguments[i]);
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

public class LoxLambdaFunction(LambdaExpr declaration, ScopeEnvironment closure) : LoxCallable
{
  public override int Arity => Declaration.Parameters.Count;
  protected override string Name => "(anonymous)";
  private readonly LambdaExpr Declaration = declaration;
  private readonly ScopeEnvironment Closure = closure;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    ScopeEnvironment environment = new(Closure);

    for (int i = 0; i < Declaration.Parameters.Count; i++)
    {
      environment.Define(Declaration.Parameters[i].Lexeme, arguments[i]);
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
