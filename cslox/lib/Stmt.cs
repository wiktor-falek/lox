public interface IStmtVisitor
{
  void VisitExprStmt(ExprStmt stmt);
  void VisitPrintStmt(PrintStmt stmt);
}

abstract public class Stmt
{
  abstract public void Accept(IStmtVisitor visitor);
}

public class ExprStmt : Stmt
{
  public readonly Expr Expression;
  public ExprStmt(Expr expression)  {
    Expression = expression;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitExprStmt(this);
  }
}

public class PrintStmt : Stmt
{
  public readonly Expr Expression;
  public PrintStmt(Expr expression)  {
    Expression = expression;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitPrintStmt(this);
  }
}
