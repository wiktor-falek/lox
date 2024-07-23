public interface IStmtVisitor
{
  void VisitIfStmt(IfStmt stmt);
  void VisitBlockStmt(BlockStmt stmt);
  void VisitExprStmt(ExprStmt stmt);
  void VisitVarStmt(VarStmt stmt);
  void VisitWhileStmt(WhileStmt stmt);
  void VisitBreakStmt(BreakStmt stmt);
  void VisitFunctionStmt(FunctionStmt stmt);
  void VisitReturnStmt(ReturnStmt stmt);
}

abstract public class Stmt
{
  abstract public void Accept(IStmtVisitor visitor);
}

public class IfStmt(Expr condition, Stmt thenBranch, List<IfStmt> elseIfStatements, Stmt? elseBranch) : Stmt
{
  public readonly Expr Condition = condition;
  public readonly Stmt ThenBranch = thenBranch;
  public readonly List<IfStmt> ElseIfStatements = elseIfStatements;
  public readonly Stmt? ElseBranch = elseBranch;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitIfStmt(this);
  }
}

public class BlockStmt(List<Stmt> statements) : Stmt
{
  public readonly List<Stmt> Statements = statements;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitBlockStmt(this);
  }
}

public class ExprStmt(Expr expression) : Stmt
{
  public readonly Expr Expression = expression;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitExprStmt(this);
  }
}

public class VarStmt(Token name, Expr? initializer) : Stmt
{
  public readonly Token Name = name;
  public readonly Expr? Initializer = initializer;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitVarStmt(this);
  }
}

public class WhileStmt(Expr expression, Stmt body) : Stmt
{
  public readonly Expr Expression = expression;
  public readonly Stmt Body = body;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitWhileStmt(this);
  }
}

public class BreakStmt(Token keyword) : Stmt
{
  public readonly Token Keyword = keyword;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitBreakStmt(this);
  }
}

public class FunctionStmt(Token name, List<Token> parameters, List<Stmt> body) : Stmt
{
  public readonly Token Name = name;
  public readonly List<Token> Parameters = parameters;
  public readonly List<Stmt> Body = body;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitFunctionStmt(this);
  }
}

public class ReturnStmt(Token keyword, Expr? value) : Stmt
{
  public readonly Token Keyword = keyword;
  public readonly Expr? Value = value;

  override public void Accept(IStmtVisitor visitor)
  {
    visitor.VisitReturnStmt(this);
  }
}
