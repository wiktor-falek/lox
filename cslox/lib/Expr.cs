abstract public class Expr {}

public class Binary : Expr
{
  readonly Expr Left;
  readonly Token Op;
  readonly Expr Right;
  Binary(Expr left, Token op, Expr right)  {
    Left = left;
    Op = op;
    Right = right;
  }
}

public class Grouping : Expr
{
  readonly Expr Expression;
  Grouping(Expr expression)  {
    Expression = expression;
  }
}

public class Literal : Expr
{
  readonly object Value;
  Literal(object value)  {
    Value = value;
  }
}

public class Unary : Expr
{
  readonly Token Op;
  readonly Expr Right;
  Unary(Token op, Expr right)  {
    Op = op;
    Right = right;
  }
}
