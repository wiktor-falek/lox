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

public class LoxFunction(FunctionStmt declaration) : LoxCallable
{
  public override int Arity => Declaration.Parameters.Count;
  protected override string Name => Declaration.Name.Lexeme;
  private readonly FunctionStmt Declaration = declaration;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    ScopeEnvironment environment = new(interpreter.Environment);
    for (int i = 0; i < Declaration.Parameters.Count; i++)
    {
      environment.Define(Declaration.Parameters[i].Lexeme, arguments[i]);
    }

    interpreter.ExecuteBlock(Declaration.Body, environment);
    return null;
  }
}

public abstract class LoxNativeFunction : LoxCallable
{
  public override string ToString()
  {
    return $"<built-in function {Name}>";
  }
}
