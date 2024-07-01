public interface IVisitor<R>
{
  R VisitBinaryExpr(Binary expr);
  R VisitGroupingExpr(Grouping expr);
  R VisitLiteralExpr(Literal expr);
  R VisitUnaryExpr(Unary expr);
  R VisitTernaryExpr(Ternary expr);
  R VisitCommaExpr(Comma expr);
}

abstract public class Expr
{
  abstract public R Accept<R>(IVisitor<R> visitor);
}

public class Binary : Expr
{
  public readonly Expr Left;
  public readonly Token Op;
  public readonly Expr Right;
  public Binary(Expr left, Token op, Expr right)  {
    Left = left;
    Op = op;
    Right = right;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitBinaryExpr(this);
  }
}

public class Grouping : Expr
{
  public readonly Expr Expression;
  public Grouping(Expr expression)  {
    Expression = expression;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitGroupingExpr(this);
  }
}

public class Literal : Expr
{
  public readonly object? Value;
  public Literal(object? value)  {
    Value = value;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLiteralExpr(this);
  }
}

public class Unary : Expr
{
  public readonly Token Op;
  public readonly Expr Right;
  public Unary(Token op, Expr right)  {
    Op = op;
    Right = right;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitUnaryExpr(this);
  }
}

public class Ternary : Expr
{
  public readonly Expr Condition;
  public readonly Expr TrueExpr;
  public readonly Expr FalseExpr;
  public Ternary(Expr condition, Expr trueExpr, Expr falseExpr)  {
    Condition = condition;
    TrueExpr = trueExpr;
    FalseExpr = falseExpr;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitTernaryExpr(this);
  }
}

public class Comma : Expr
{
  public readonly List<Expr> Expressions;
  public Comma(List<Expr> expressions)  {
    Expressions = expressions;
  }
  override public R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitCommaExpr(this);
  }
}
