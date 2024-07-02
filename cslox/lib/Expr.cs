public interface IExprVisitor<R>
{
  R VisitAssignExpr(AssignExpr expr);
  R VisitBinaryExpr(BinaryExpr expr);
  R VisitGroupingExpr(GroupingExpr expr);
  R VisitLiteralExpr(LiteralExpr expr);
  R VisitUnaryExpr(UnaryExpr expr);
  R VisitTernaryExpr(TernaryExpr expr);
  R VisitCommaExpr(CommaExpr expr);
  R VisitVariableExpr(VariableExpr expr);
}

abstract public class Expr
{
  abstract public R Accept<R>(IExprVisitor<R> visitor);
}

public class AssignExpr : Expr
{
  public readonly Token Name;
  public readonly Expr Value;
  public AssignExpr(Token name, Expr value)  {
    Name = name;
    Value = value;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitAssignExpr(this);
  }
}

public class BinaryExpr : Expr
{
  public readonly Expr Left;
  public readonly Token Op;
  public readonly Expr Right;
  public BinaryExpr(Expr left, Token op, Expr right)  {
    Left = left;
    Op = op;
    Right = right;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitBinaryExpr(this);
  }
}

public class GroupingExpr : Expr
{
  public readonly Expr Expression;
  public GroupingExpr(Expr expression)  {
    Expression = expression;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitGroupingExpr(this);
  }
}

public class LiteralExpr : Expr
{
  public readonly object? Value;
  public LiteralExpr(object? value)  {
    Value = value;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitLiteralExpr(this);
  }
}

public class UnaryExpr : Expr
{
  public readonly Token Op;
  public readonly Expr Right;
  public UnaryExpr(Token op, Expr right)  {
    Op = op;
    Right = right;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitUnaryExpr(this);
  }
}

public class TernaryExpr : Expr
{
  public readonly Expr Condition;
  public readonly Expr TrueExpr;
  public readonly Expr FalseExpr;
  public TernaryExpr(Expr condition, Expr trueExpr, Expr falseExpr)  {
    Condition = condition;
    TrueExpr = trueExpr;
    FalseExpr = falseExpr;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitTernaryExpr(this);
  }
}

public class CommaExpr : Expr
{
  public readonly List<Expr> Expressions;
  public CommaExpr(List<Expr> expressions)  {
    Expressions = expressions;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitCommaExpr(this);
  }
}

public class VariableExpr : Expr
{
  public readonly Token Name;
  public VariableExpr(Token name)  {
    Name = name;
  }
  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitVariableExpr(this);
  }
}
