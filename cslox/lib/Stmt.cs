public interface IStmtVisitor
{
  void VisitIfStmt(IfStmt stmt);
  void VisitBlockStmt(BlockStmt stmt);
  void VisitExprStmt(ExprStmt stmt);
  void VisitPrintStmt(PrintStmt stmt);
  void VisitVarStmt(VarStmt stmt);
  void VisitWhileStmt(WhileStmt stmt);
  void VisitBreakStmt(BreakStmt stmt);
  void VisitFunctionStmt(FunctionStmt stmt);
}

abstract public class Stmt
{
  abstract public void Accept(IStmtVisitor visitor);
}

public class IfStmt : Stmt
{
  public readonly Expr Condition;
  public readonly Stmt ThenBranch;
  public readonly Stmt? ElseBranch;
  public IfStmt(Expr condition, Stmt thenBranch, Stmt? elseBranch)  {
    Condition = condition;
    ThenBranch = thenBranch;
    ElseBranch = elseBranch;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitIfStmt(this);
  }
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

public class WhileStmt : Stmt
{
  public readonly Expr Expression;
  public readonly Stmt Body;
  public WhileStmt(Expr expression, Stmt body)  {
    Expression = expression;
    Body = body;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitWhileStmt(this);
  }
}

public class BreakStmt : Stmt
{
  public readonly Token Token;
  public BreakStmt(Token token)  {
    Token = token;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitBreakStmt(this);
  }
}

public class FunctionStmt : Stmt
{
  public readonly Token Name;
  public readonly List<Token> Parameters;
  public readonly List<Stmt> Body;
  public FunctionStmt(Token name, List<Token> parameters, List<Stmt> body)  {
    Name = name;
    Parameters = parameters;
    Body = body;
  }
  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitFunctionStmt(this);
  }
}
