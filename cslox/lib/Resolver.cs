public struct Void { }

class Resolver(Interpreter interpreter) : IExprVisitor<Void>, IStmtVisitor
{
  private readonly Interpreter Interpreter = interpreter;
  private readonly Stack<Dictionary<string, bool>> Scopes = new();

  public void Resolve(List<Stmt> statements)
  {
    foreach (Stmt statement in statements)
    {
      Resolve(statement);
    }
  }

  private void Resolve(Stmt statement)
  {
    statement.Accept(this);
  }

  private void Resolve(Expr expression)
  {
    expression.Accept(this);
  }

  private void ResolveFunction(FunctionStmt function)
  {
    BeginScope();
    foreach (Token parameter in function.Parameters)
    {
      Declare(parameter);
      Define(parameter);
    }
    Resolve(function.Body);
    EndScope();
  }

  private void Declare(Token name)
  {
    if (Scopes.Count == 0) return;
    Scopes.Peek()[name.Lexeme] = false;
  }

  private void Define(Token name)
  {
    if (Scopes.Count == 0) return;
    Scopes.Peek()[name.Lexeme] = true;
  }

  private void ResolveLocal(Expr expression, Token name)
  {
    for (int i = Scopes.Count - 1; i >= 0; i--)
    {
      if (Scopes.ElementAt(1).ContainsKey(name.Lexeme))
      {
        Interpreter.Resolve(expression, Scopes.Count - 1 - i);
      }
    }
  }

  private void BeginScope()
  {
    Scopes.Push([]);
  }

  private void EndScope()
  {
    Scopes.Pop();
  }

  Void IExprVisitor<Void>.VisitAssignExpr(AssignExpr expr)
  {
    Resolve(expr.Value);
    ResolveLocal(expr, expr.Name);
    return default;
  }

  Void IExprVisitor<Void>.VisitBinaryExpr(BinaryExpr expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);
    return default;
  }

  Void IExprVisitor<Void>.VisitGroupingExpr(GroupingExpr expr)
  {
    Resolve(expr.Expression);
    return default;
  }

  Void IExprVisitor<Void>.VisitLiteralExpr(LiteralExpr expr)
  {
    return default;
  }

  Void IExprVisitor<Void>.VisitLogicalExpr(LogicalExpr expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);
    return default;
  }

  Void IExprVisitor<Void>.VisitUnaryExpr(UnaryExpr expr)
  {
    Resolve(expr.Right);
    return default;
  }

  Void IExprVisitor<Void>.VisitTernaryExpr(TernaryExpr expr)
  {
    Resolve(expr.Condition);
    Resolve(expr.TrueExpr);
    Resolve(expr.FalseExpr);
    return default;
  }

  Void IExprVisitor<Void>.VisitCommaExpr(CommaExpr expr)
  {
    foreach (Expr expression in expr.Expressions)
    {
      Resolve(expression);
    }
    return default;
  }

  Void IExprVisitor<Void>.VisitVariableExpr(VariableExpr expr)
  {
    if (Scopes.Count != 0 && Scopes.Peek()[expr.Name.Lexeme] == false)
    {
      Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
    }

    ResolveLocal(expr, expr.Name);
    return default;
  }

  Void IExprVisitor<Void>.VisitCallExpr(CallExpr expr)
  {
    Resolve(expr.Callee);

    foreach (Expr argument in expr.Arguments)
    {
      Resolve(argument);
    }

    return default;
  }

  Void IExprVisitor<Void>.VisitLambdaExpr(LambdaExpr expr)
  {
    BeginScope();
    foreach (Token parameter in expr.Parameters)
    {
      Declare(parameter);
      Define(parameter);
    }
    Resolve(expr.Body);
    EndScope();
    return default;
  }

  // Void IExprVisitor<Void>.VisitLogicalExpr(LogicalExpr expr)
  // {
  //   Resolve(expr.Left);
  //   Resolve(expr.Right);
  //   return default;
  // }

  public void VisitIfStmt(IfStmt stmt)
  {
    Resolve(stmt.Condition);
    Resolve(stmt.ThenBranch);
    if (stmt.ElseBranch is not null) Resolve(stmt.ElseBranch);
  }

  public void VisitBlockStmt(BlockStmt stmt)
  {
    BeginScope();
    Resolve(stmt.Statements);
    EndScope();
  }

  public void VisitExprStmt(ExprStmt stmt)
  {
    Resolve(stmt.Expression);
  }

  public void VisitVarStmt(VarStmt stmt)
  {
    Declare(stmt.Name);
    if (stmt.Initializer is not null)
    {
      Resolve(stmt.Initializer);
    }
    Define(stmt.Name);
  }

  public void VisitWhileStmt(WhileStmt stmt)
  {
    Resolve(stmt.Expression);
    Resolve(stmt.Body);
  }

  public void VisitBreakStmt(BreakStmt stmt) { }

  public void VisitFunctionStmt(FunctionStmt stmt)
  {
    Declare(stmt.Name);
    Define(stmt.Name);

    ResolveFunction(stmt);
  }

  public void VisitReturnStmt(ReturnStmt stmt)
  {
    if (stmt.Value is not null)
    {
      Resolve(stmt.Value);
    }
  }
}
