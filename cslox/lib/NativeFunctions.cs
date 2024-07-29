public abstract class LoxNativeFunction : LoxCallable
{
  public override string ToString()
  {
    return $"<built-in function {Name}>";
  }
}

public class PrintNativeFunction : LoxNativeFunction
{
  public override int Arity => 1;
  public override string Name => "print";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    Console.WriteLine(Interpreter.Stringify(arguments[0]));
    return null;
  }
}

public class InputNativeFunction : LoxNativeFunction
{
  public override int Arity => 0;
  public override string Name => "input";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return Console.ReadLine();
  }
}

public class ClockNativeFunction : LoxNativeFunction
{
  public override string Name => "clock";
  public override int Arity => 0;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}

public class IntNativeFunction : LoxNativeFunction
{
  public override int Arity => 1;
  public override string Name => "int";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    if (arguments[0] is double num)
    {
      return (double)(int)num;
    }

    throw new NotImplementedException();
  }
}

public class RandNativeFunction : LoxNativeFunction
{
  public override int Arity => 0;
  public override string Name => "rand";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    return new Random().NextDouble();
  }
}

public class ExitNativeFunction : LoxNativeFunction
{
  public override int Arity => 1;
  public override string Name => "exit";

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    if (arguments[0] is double code)
    {
      Environment.Exit((int)code);
      return null;
    }

    throw new NotImplementedException();
  }
}
