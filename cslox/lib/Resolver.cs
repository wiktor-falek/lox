public struct Void { };

class Resolver(Interpreter interpreter) : IExprVisitor<Void>, IStmtVisitor
{
  private readonly Interpreter Interpreter = interpreter;
  private readonly Stack<Dictionary<string, bool>> Scopes = [];

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

  private void BeginScope()
  {
    Scopes.Push([]);
  }

  private void EndScope()
  {
    Scopes.Pop();
  }

  private void ResolveLocal(Expr expr, Token name)
  {
    for (int i = Scopes.Count; i >= 0; i--)
    {
      if (Scopes.ElementAt(i).ContainsKey(name.Lexeme))
      {
        Interpreter.Resolve(expr, Scopes.Count - 1 - i);
        return;
      }
    }
  }

  private void Declare(Token name)
  {
    if (Scopes.TryPeek(out var scope))
    {
      scope.Add(name.Lexeme, false);
    }
  }

  private void Define(Token name)
  {
    if (Scopes.TryPeek(out var scope))
    {
      scope.Remove(name.Lexeme);
      scope.Add(name.Lexeme, true);
    }
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

  private void ResolveFunction(LambdaExpr lambda)
  {
    BeginScope();
    foreach (Token parameter in lambda.Parameters)
    {
      Declare(parameter);
      Define(parameter);
    }
    Resolve(lambda.Body);
    EndScope();
  }

  public Void VisitAssignExpr(AssignExpr expr)
  {
    Resolve(expr.Value);
    ResolveLocal(expr, expr.Name);
    return default;
  }

  public Void VisitBinaryExpr(BinaryExpr expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);
    return default;
  }

  public Void VisitCallExpr(CallExpr expr)
  {
    Resolve(expr.Callee);

    foreach (Expr argument in expr.Arguments)
    {
      Resolve(argument);
    }

    return default;
  }

  public Void VisitCommaExpr(CommaExpr expr)
  {
    foreach (Expr expression in expr.Expressions)
    {
      Resolve(expression);
    }

    return default;
  }

  public Void VisitGroupingExpr(GroupingExpr expr)
  {
    Resolve(expr.Expression);
    return default;
  }

  public Void VisitLambdaExpr(LambdaExpr expr)
  {
    ResolveFunction(expr);
    return default;
  }

  public Void VisitLiteralExpr(LiteralExpr expr)
  {
    return default;
  }

  public Void VisitLogicalExpr(LogicalExpr expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);
    return default;
  }

  public Void VisitTernaryExpr(TernaryExpr expr)
  {
    Resolve(expr.Condition);
    Resolve(expr.TrueExpr);
    Resolve(expr.FalseExpr);
    return default;
  }

  public Void VisitUnaryExpr(UnaryExpr expr)
  {
    Resolve(expr.Right);
    return default;
  }

  public Void VisitVariableExpr(VariableExpr expr)
  {
    if (Scopes.TryPeek(out var scope)
        && scope.TryGetValue(expr.Name.Lexeme, out var variable)
        && variable == false)
    {
      Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
    }

    ResolveLocal(expr, expr.Name);
    return default;
  }

  public void VisitBlockStmt(BlockStmt stmt)
  {
    BeginScope();
    Resolve(stmt.Statements);
    EndScope();
  }

  public void VisitBreakStmt(BreakStmt stmt) { }

  public void VisitExprStmt(ExprStmt stmt)
  {
    Resolve(stmt.Expression);
  }

  public void VisitFunctionStmt(FunctionStmt stmt)
  {
    Declare(stmt.Name);
    Define(stmt.Name);
    ResolveFunction(stmt);
  }

  public void VisitIfStmt(IfStmt stmt)
  {
    Resolve(stmt.Condition);
    Resolve(stmt.ThenBranch);

    foreach (IfStmt elseIfStmt in stmt.ElseIfStatements)
    {
      Resolve(elseIfStmt);
    }

    if (stmt.ElseBranch is not null)
    {
      Resolve(stmt.ElseBranch);
    }
  }

  public void VisitReturnStmt(ReturnStmt stmt)
  {
    if (stmt.Value is not null)
    {
      Resolve(stmt.Value);
    }
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
    Resolve(stmt.Condition);
    Resolve(stmt.Body);
  }
}
