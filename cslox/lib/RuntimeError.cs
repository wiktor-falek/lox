public class RuntimeError(Token token, string message) : SystemException(message)
{
  public readonly Token Token = token;
}

public class Break(Token token) : RuntimeError(token, "Cannot break outside of a loop.");

public class Return(Token token, object? value) : RuntimeError(token, "Cannot return outside of a function.")
{
  public readonly object? Value = value;
}
