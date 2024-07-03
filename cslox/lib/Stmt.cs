public interface IStmtVisitor
{
  void VisitBlockStmt(BlockStmt stmt);
  void VisitExprStmt(ExprStmt stmt);
  void VisitPrintStmt(PrintStmt stmt);
  void VisitVarStmt(VarStmt stmt);
}

abstract public class Stmt
{
  abstract public void Accept(IStmtVisitor visitor);
}

public class BlockStmt : Stmt
{
  public readonly List<Stmt> Statements;
  public BlockStmt(List<Stmt> statements)  {
    Statements = statements;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitBlockStmt(this);
  }
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

public class VarStmt : Stmt
{
  public readonly Token Name;
  public readonly Expr? Initializer;
  public VarStmt(Token name, Expr? initializer)  {
    Name = name;
    Initializer = initializer;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitVarStmt(this);
  }
}
