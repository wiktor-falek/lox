public class ClockNativeFunction : LoxNativeFunction
{
  protected override string Name => "clock";
  public override int Arity => 0;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}
