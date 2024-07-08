public class ClockNativeFunction : LoxNativeFunction
{
  protected override string Name => "clock";
  public override int Arity => 0;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}

public class PrintNativeFunction : LoxNativeFunction
{
  public override int Arity => 1;
  protected override string Name => "print";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    Console.WriteLine(Interpreter.Stringify(arguments[0]));
    return null;
  }
}