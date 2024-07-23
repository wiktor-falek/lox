public interface IExprVisitor<R>
{
  R VisitAssignExpr(AssignExpr expr);
  R VisitBinaryExpr(BinaryExpr expr);
  R VisitGroupingExpr(GroupingExpr expr);
  R VisitLiteralExpr(LiteralExpr expr);
  R VisitLogicalExpr(LogicalExpr expr);
  R VisitUnaryExpr(UnaryExpr expr);
  R VisitTernaryExpr(TernaryExpr expr);
  R VisitCommaExpr(CommaExpr expr);
  R VisitVariableExpr(VariableExpr expr);
  R VisitCallExpr(CallExpr expr);
  R VisitLambdaExpr(LambdaExpr expr);
}

abstract public class Expr
{
  abstract public R Accept<R>(IExprVisitor<R> visitor);
}

public class AssignExpr(Token name, Expr value) : Expr
{
  public readonly Token Name = name;
  public readonly Expr Value = value;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitAssignExpr(this);
  }
}

public class BinaryExpr(Expr left, Token op, Expr right) : Expr
{
  public readonly Expr Left = left;
  public readonly Token Op = op;
  public readonly Expr Right = right;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitBinaryExpr(this);
  }
}

public class GroupingExpr(Expr expression) : Expr
{
  public readonly Expr Expression = expression;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitGroupingExpr(this);
  }
}

public class LiteralExpr(object? value) : Expr
{
  public readonly object? Value = value;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitLiteralExpr(this);
  }
}

public class LogicalExpr(Expr left, Token op, Expr right) : Expr
{
  public readonly Expr Left = left;
  public readonly Token Op = op;
  public readonly Expr Right = right;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitLogicalExpr(this);
  }
}

public class UnaryExpr(Token op, Expr right) : Expr
{
  public readonly Token Op = op;
  public readonly Expr Right = right;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitUnaryExpr(this);
  }
}

public class TernaryExpr(Expr condition, Expr trueExpr, Expr falseExpr) : Expr
{
  public readonly Expr Condition = condition;
  public readonly Expr TrueExpr = trueExpr;
  public readonly Expr FalseExpr = falseExpr;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitTernaryExpr(this);
  }
}

public class CommaExpr(List<Expr> expressions) : Expr
{
  public readonly List<Expr> Expressions = expressions;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitCommaExpr(this);
  }
}

public class VariableExpr(Token name) : Expr
{
  public readonly Token Name = name;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitVariableExpr(this);
  }
}

public class CallExpr(Expr callee, Token paren, List<Expr> arguments) : Expr
{
  public readonly Expr Callee = callee;
  public readonly Token Paren = paren;
  public readonly List<Expr> Arguments = arguments;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitCallExpr(this);
  }
}

public class LambdaExpr(List<Token> parameters, List<Stmt> body) : Expr
{
  public readonly List<Token> Parameters = parameters;
  public readonly List<Stmt> Body = body;

  override public R Accept<R>(IExprVisitor<R> visitor)
  {
    return visitor.VisitLambdaExpr(this);
  }
}
