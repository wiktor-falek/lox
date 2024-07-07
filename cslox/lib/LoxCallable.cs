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

public abstract class LoxNativeFunction : LoxCallable
{
  public override string ToString()
  {
    return $"<built-in function {Name}>";
  }
}

class ClockNativeFunction : LoxNativeFunction
{
  protected override string Name => "clock";

  public override int Arity => 0;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}
