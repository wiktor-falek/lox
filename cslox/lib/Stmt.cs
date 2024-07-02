public interface IStmtVisitor
{
  void VisitExprStmt(ExprStmt stmt);
  void VisitPrintStmt(PrintStmt stmt);
}

public interface IStmtVisitor<R>
{
  R VisitExprStmt(ExprStmt stmt);
  R VisitPrintStmt(PrintStmt stmt);
}

abstract public class Stmt
{
  abstract public R Accept<R>(IStmtVisitor<R> visitor);
}

public class ExprStmt : Stmt
{
  public readonly Expr Expression;
  public ExprStmt(Expr expression)  {
    Expression = expression;
  }
  override public R Accept<R>(IStmtVisitor<R> visitor)
  {
    return visitor.VisitExprStmt(this);
  }
}

public class PrintStmt : Stmt
{
  public readonly Expr Expression;
  public PrintStmt(Expr expression)  {
    Expression = expression;
  }
  override public R Accept<R>(IStmtVisitor<R> visitor)
  {
    return visitor.VisitPrintStmt(this);
  }
}
