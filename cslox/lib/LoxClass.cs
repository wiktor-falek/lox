
class LoxClass(string name) : LoxCallable
{
  public override string Name => name;
  public override int Arity => 0;

  public override object? Call(Interpreter interpreter, List<object?> arguments)
  {
    LoxInstance instance = new(this);
    return instance;
  }

  public override string ToString()
  {
    return $"<class {Name}>";
  }
}
