using static TokenType;

public class RuntimeError(Token token, string message) : SystemException(message)
{
  public readonly Token Token = token;
}

public class Break(Token token) : RuntimeError(token, "Cannot break outside of a loop.");

public class Return(Token token, object? value) : RuntimeError(token, "Cannot return outside of a function.")
{
  public readonly object? Value = value;
}

public class Interpreter : IExprVisitor<object?>, IStmtVisitor
{
  public readonly ScopeEnvironment Globals;
  public ScopeEnvironment Environment;
  public Option<object?> LastExpressionValue = Option<object?>.None();

  public Interpreter()
  {
    Globals = Environment = new ScopeEnvironment();

    Globals.Define("print", new PrintNativeFunction());
    Globals.Define("input", new InputNativeFunction());
    Globals.Define("clock", new ClockNativeFunction());
    Globals.Define("int", new IntNativeFunction());
    Globals.Define("rand", new RandNativeFunction());
    Globals.Define("exit", new ExitNativeFunction());
  }

  public void Interpret(List<Stmt> statements)
  {
    try
    {
      foreach (Stmt statement in statements)
      {
        Execute(statement);
      }
    }
    catch (RuntimeError error)
    {
      Lox.RuntimeError(error);
    }
  }

  private void Execute(Stmt statement)
  {
    statement.Accept(this);
  }

  void IStmtVisitor.VisitExprStmt(ExprStmt stmt)
  {
    object? value = Evaluate(stmt.Expression);
    LastExpressionValue = Option<object?>.Some(value);
  }

  void IStmtVisitor.VisitIfStmt(IfStmt stmt)
  {
    if (IsTruthy(Evaluate(stmt.Condition)))
    {
      Execute(stmt.ThenBranch);
    }
    else if (stmt.ElseBranch is not null)
    {
      Execute(stmt.ElseBranch);
    };
  }

  void IStmtVisitor.VisitWhileStmt(WhileStmt stmt)
  {
    while (IsTruthy(Evaluate(stmt.Expression)))
    {
      try
      {
        Execute(stmt.Body);
      }
      catch (Break)
      {
        break;
      }
    }
  }

  void IStmtVisitor.VisitBreakStmt(BreakStmt stmt)
  {
    throw new Break(stmt.Token);
  }

  void IStmtVisitor.VisitReturnStmt(ReturnStmt stmt)
  {
    object? value = null;
    if (stmt.Value is not null)
    {
      value = Evaluate(stmt.Value);
    }

    throw new Return(stmt.Keyword, value);
  }

  void IStmtVisitor.VisitFunctionStmt(FunctionStmt stmt)
  {
    LoxFunction function = new(stmt);
    Environment.Define(stmt.Name.Lexeme, function);
  }

  void IStmtVisitor.VisitVarStmt(VarStmt stmt)
  {
    object? value = null;

    if (stmt.Initializer is not null)
    {
      value = Evaluate(stmt.Initializer);
    }

    Environment.Define(stmt.Name.Lexeme, value);
  }

  void IStmtVisitor.VisitBlockStmt(BlockStmt stmt)
  {
    ExecuteBlock(stmt.Statements, new ScopeEnvironment(Environment));
  }

  public void ExecuteBlock(List<Stmt> statements, ScopeEnvironment environment)
  {
    ScopeEnvironment previous = Environment;

    try
    {
      Environment = environment;

      foreach (Stmt statement in statements)
      {
        Execute(statement);
      }
    }
    finally
    {
      Environment = previous;
    }
  }

  object? IExprVisitor<object?>.VisitAssignExpr(AssignExpr expr)
  {
    object? value = Evaluate(expr.Value);
    Environment.Assign(expr.Name, value);
    return value;
  }

  object? IExprVisitor<object?>.VisitLiteralExpr(LiteralExpr expr)
  {
    return expr.Value;
  }

  object? IExprVisitor<object?>.VisitGroupingExpr(GroupingExpr expr)
  {
    return Evaluate(expr.Expression);
  }

  object? IExprVisitor<object?>.VisitUnaryExpr(UnaryExpr expr)
  {
    object? right = Evaluate(expr.Right);

    switch (expr.Op.Type)
    {
      case MINUS:
        CheckNumberOperand(expr.Op, right);
        return -(double)right!;
      case BANG:
        return !IsTruthy(right);
      default:
        return null;
    };
  }

  object? IExprVisitor<object?>.VisitBinaryExpr(BinaryExpr expr)
  {
    object? left = Evaluate(expr.Left);
    object? right = Evaluate(expr.Right);

    switch (expr.Op.Type)
    {
      case PLUS:
        if (left is double ld && right is double rd)
        {
          return ld + rd;
        }
        else if (left is string ls && right is string rs)
        {
          return ls + rs;
        }
        else if (left is string && right is double)
        {
          return left + right.ToString();
        }
        else if (left is double && right is string)
        {
          return left.ToString() + right;
        }
        throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");
      case MINUS:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! - (double)right!;
      case STAR:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! * (double)right!;
      case SLASH:
        CheckNumberOperands(expr.Op, left, right);
        if ((double)right! == 0) throw new RuntimeError(expr.Op, "Cannot divide by 0.");
        return (double)left! / (double)right!;
      case GREATER:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! > (double)right!;
      case GREATER_EQUAL:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! >= (double)right!;
      case LESS:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! < (double)right!;
      case LESS_EQUAL:
        CheckNumberOperands(expr.Op, left, right);
        return (double)left! <= (double)right!;
      case EQUAL_EQUAL:
        return IsEqual(left, right);
      case BANG_EQUAL:
        return !IsEqual(left, right);
      default:
        return null;
    }
  }

  object? IExprVisitor<object?>.VisitTernaryExpr(TernaryExpr expr)
  {
    if (IsTruthy(Evaluate(expr.Condition)))
    {
      return Evaluate(expr.TrueExpr);
    }
    return Evaluate(expr.FalseExpr);
  }

  object? IExprVisitor<object?>.VisitCommaExpr(CommaExpr expr)
  {
    object? lastExprValue = null;

    foreach (Expr expression in expr.Expressions)
    {
      lastExprValue = Evaluate(expression);
    }

    return lastExprValue;
  }

  object? IExprVisitor<object?>.VisitVariableExpr(VariableExpr expr)
  {
    return Environment.Get(expr.Name);
  }

  object? IExprVisitor<object?>.VisitCallExpr(CallExpr expr)
  {
    object? callee = Evaluate(expr.Callee);

    List<object?> arguments = [];
    foreach (Expr argument in expr.Arguments)
    {
      arguments.Add(Evaluate(argument));
    }

    if (callee is not LoxCallable function)
    {
      throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
    }

    if (arguments.Count != function.Arity)
    {
      throw new RuntimeError(
        expr.Paren,
        $"Expected {function.Arity} arguments but got {arguments.Count}."
      );
    }

    return function.Call(this, arguments);
  }

  public static string Stringify(object? obj)
  {
    if (obj is null) return "nil";
    if (obj is double)
    {
      string text = obj.ToString() ?? "nil";
      if (text.EndsWith(".0"))
      {
        text = text[..^2];
      }
      return text;
    }
    if (obj is string) return '"' + obj.ToString() + '"';

    return obj.ToString() ?? "nil";
  }

  private object? Evaluate(Expr expr)
  {
    return expr.Accept(this);
  }

  private static bool IsTruthy(object? obj)
  {
    if (obj is null) return false;
    else if (obj is bool v) return v;
    else if (obj is double d) return d != 0;
    else if (obj is string s) return s.Length != 0;
    return true;
  }

  private static bool IsEqual(object? a, object? b)
  {
    if (a is null && b is null) return true;
    if (a is null) return false;
    // TODO: 
    // string and bool
    // number and bool

    return a.Equals(b);
  }

  private static void CheckNumberOperand(Token op, object? operand)
  {
    if (operand is double) return;
    throw new RuntimeError(op, "Operand must be a number.");
  }

  private static void CheckNumberOperands(Token op, object? left, object? right)
  {
    if (left is double && right is double) return;

    throw new RuntimeError(op, "Operands must be numbers.");
  }
}
