public struct Void { }

class Resolver(Interpreter interpreter) : IExprVisitor<Void>, IStmtVisitor
{
  private readonly Interpreter Interpreter = interpreter;
  private readonly Stack<Dictionary<string, Variable>> Scopes = [];
  private FunctionType CurrentFunction = FunctionType.NONE;
  private bool IsInLoop = false;
  private readonly List<(int line, string message)> UnusedVariableWarnings = [];

  public void Resolve(List<Stmt> statements)
  {
    foreach (Stmt statement in statements)
    {
      Resolve(statement);
    }

    var sortedWarnings = UnusedVariableWarnings.OrderBy(e => e.line);
    foreach (var (line, message) in sortedWarnings)
    {
      Lox.Warn(line, message);
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
    var scope = Scopes.Pop();

    foreach (var (_, variable) in scope)
    {
      if (variable.State == VariableState.DEFINED)
      {
        UnusedVariableWarnings.Add((variable.Name.Line, $"Unused local variable '{variable.Name.Lexeme}'."));
      }
    }
  }

  private void ResolveLocal(Expr expr, Token name, bool isRead)
  {
    for (int i = Scopes.Count - 1; i >= 0; i--)
    {
      var scope = Scopes.ElementAt(i);
      if (scope.TryGetValue(name.Lexeme, out Variable value))
      {
        int slot = scope[name.Lexeme].Slot;
        Interpreter.Resolve(expr, distance: i, slot);

        if (isRead)
        {
          scope.Remove(name.Lexeme);
          scope.Add(name.Lexeme, new Variable(value.Name, slot, VariableState.READ));
        }
        return;
      }
    }
  }

  private void Declare(Token name)
  {
    if (Scopes.TryPeek(out var scope))
    {
      int slot = scope.Count;
      if (!scope.TryAdd(name.Lexeme, new Variable(name, slot, VariableState.DECLARED)))
      {
        Lox.Error(name, "Already a variable with this name in this scope.");
      }
    }
  }

  private void Define(Token name)
  {
    if (Scopes.TryPeek(out var scope))
    {
      int slot = scope[name.Lexeme].Slot;
      scope.Remove(name.Lexeme);
      scope.Add(name.Lexeme, new Variable(name, slot, VariableState.DEFINED));
    }
  }

  private void ResolveFunction(FunctionStmt function, FunctionType type)
  {
    FunctionType enclosingFunction = CurrentFunction;
    CurrentFunction = type;

    BeginScope();
    foreach (Token parameter in function.Parameters)
    {
      Declare(parameter);
      Define(parameter);
    }
    foreach (var statement in function.Body)
    {
      Resolve(statement);
    }
    EndScope();

    CurrentFunction = enclosingFunction;
  }

  private void ResolveFunction(LambdaExpr lambda)
  {
    FunctionType enclosingFunction = CurrentFunction;
    CurrentFunction = FunctionType.FUNCTION;

    BeginScope();
    foreach (Token parameter in lambda.Parameters)
    {
      Declare(parameter);
      Define(parameter);
    }
    foreach (var statement in lambda.Body)
    {
      Resolve(statement);
    }
    EndScope();

    CurrentFunction = enclosingFunction;
  }

  public Void VisitAssignExpr(AssignExpr expr)
  {
    Resolve(expr.Value);
    ResolveLocal(expr, expr.Name, false);
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
        && variable.State == VariableState.DECLARED)
    {
      Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
    }

    ResolveLocal(expr, expr.Name, true);
    return default;
  }

  public void VisitBlockStmt(BlockStmt stmt)
  {
    BeginScope();
    foreach (var statement in stmt.Statements)
    {
      Resolve(statement);
    }
    EndScope();
  }

  public void VisitBreakStmt(BreakStmt stmt)
  {
    if (!IsInLoop)
    {
      Lox.Error(stmt.Keyword, "Can't break outside of a loop.");
    }
  }

  public void VisitExprStmt(ExprStmt stmt)
  {
    Resolve(stmt.Expression);
  }

  public void VisitFunctionStmt(FunctionStmt stmt)
  {
    Declare(stmt.Name);
    Define(stmt.Name);
    ResolveFunction(stmt, FunctionType.FUNCTION);
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
    if (CurrentFunction is FunctionType.NONE)
    {
      Lox.Error(stmt.Keyword, "Can't return from top-level code.");
    }

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
    bool previousLoopState = IsInLoop;
    IsInLoop = true;

    Resolve(stmt.Condition);
    Resolve(stmt.Body);

    IsInLoop = previousLoopState;
  }

  private struct Variable(Token name, int slot, VariableState state)
  {
    public readonly Token Name = name;
    public readonly int Slot = slot;
    public VariableState State = state;
  }

  private enum VariableState
  {
    DECLARED,
    DEFINED,
    READ
  }

  private enum FunctionType
  {
    NONE,
    FUNCTION,
  }
}
