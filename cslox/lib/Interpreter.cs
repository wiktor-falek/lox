using static TokenType;

public class Interpreter : IExprVisitor<object?>, IStmtVisitor
{
  private readonly Dictionary<string, object?> Globals = [];
  public readonly ScopeEnvironment Global; // alternative to nullable, never written to or read from
  private ScopeEnvironment Environment;
  private readonly Dictionary<Expr, (int distance, int slot)> Locals = [];
  public Option<object?> LastExpressionValue = Option<object?>.None();

  public Interpreter()
  {
    Environment = Global = new();
    Globals.Add("print", new PrintNativeFunction());
    Globals.Add("input", new InputNativeFunction());
    Globals.Add("clock", new ClockNativeFunction());
    Globals.Add("int", new IntNativeFunction());
    Globals.Add("rand", new RandNativeFunction());
    Globals.Add("exit", new ExitNativeFunction());
  }

  public void Define(Token name, object? value)
  {
    if (Environment.Equals(Global))
    {
      Globals.Add(name.Lexeme, value);
    }
    else
    {
      Environment.Define(value);
    }
  }

  public void Resolve(Expr expression, int distance, int slot)
  {
    Locals.Add(expression, (distance, slot));
  }

  public object? LookUpVariable(Token name, Expr expr)
  {
    if (Locals.TryGetValue(expr, out var local))
    {
      return Environment.GetAt(local.distance, local.slot);
    }

    if (Globals.TryGetValue(name.Lexeme, out var value))
    {
      return value;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
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
    List<IfStmt> ifStatements = [stmt, .. stmt.ElseIfStatements];

    bool executedIfStmt = false;

    foreach (var ifStatement in ifStatements)
    {
      if (IsTruthy(Evaluate(ifStatement.Condition)))
      {
        Execute(ifStatement.ThenBranch);
        executedIfStmt = true;
        break;
      }
    }

    if (!executedIfStmt && stmt.ElseBranch is not null)
    {
      Execute(stmt.ElseBranch);
    }
  }

  void IStmtVisitor.VisitWhileStmt(WhileStmt stmt)
  {
    while (IsTruthy(Evaluate(stmt.Condition)))
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
    throw new Break(stmt.Keyword);
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
    LoxFunction function = new(stmt, Environment, isInitializer: false);
    Define(stmt.Name, function);
  }

  void IStmtVisitor.VisitClassStmt(ClassStmt stmt)
  {
    Dictionary<string, LoxFunction> methods = [];
    foreach (var method in stmt.Methods)
    {
      LoxFunction function = new(method, Environment, isInitializer: method.Name.Lexeme == "init");
      methods.Add(method.Name.Lexeme, function);
    }

    LoxClass @class = new(stmt.Name.Lexeme, methods);
    Define(stmt.Name, @class);
  }

  void IStmtVisitor.VisitVarStmt(VarStmt stmt)
  {
    object? value = null;

    if (stmt.Initializer is not null)
    {
      value = Evaluate(stmt.Initializer);
    }

    Define(stmt.Name, value);
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

    if (Locals.TryGetValue(expr, out var local))
    {
      var (distance, slot) = local;
      Environment.AssignAt(distance, slot, value);
    }
    else
    {
      Globals.Remove(expr.Name.Lexeme);
      Globals.Add(expr.Name.Lexeme, value);
    }

    return value;
  }

  object? IExprVisitor<object?>.VisitLiteralExpr(LiteralExpr expr)
  {
    return expr.Value;
  }

  object? IExprVisitor<object?>.VisitLogicalExpr(LogicalExpr expr)
  {
    object? left = Evaluate(expr.Left);

    if (expr.Op.Type == OR)
    {
      if (IsTruthy(left)) return left;
    }
    else
    {
      if (!IsTruthy(left)) return left;
    }

    return Evaluate(expr.Right);
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
    return LookUpVariable(expr.Name, expr);
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

  object? IExprVisitor<object?>.VisitGetExpr(GetExpr expr)
  {
    object? obj = Evaluate(expr.Obj);

    if (obj is LoxInstance instance)
    {
      return instance.Get(expr.Name);
    }

    throw new RuntimeError(expr.Name, "Only instances have properties.");
  }

  object? IExprVisitor<object?>.VisitSetExpr(SetExpr expr)
  {
    object? obj = Evaluate(expr.Obj);

    if (obj is LoxInstance instance)
    {
      object? value = Evaluate(expr.Value);
      instance.Set(expr.Name, value);
      return value;
    }

    throw new RuntimeError(expr.Name, "Only instances have fields.");
  }

  object? IExprVisitor<object?>.VisitThisExpr(ThisExpr expr)
  {
    return LookUpVariable(expr.Keyword, expr);
  }

  object? IExprVisitor<object?>.VisitLambdaExpr(LambdaExpr expr)
  {
    return new LoxLambdaFunction(expr, Environment);
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
    else if (obj is bool b) return b;
    else if (obj is double d) return d != 0;
    else if (obj is string s) return s.Length != 0;
    return true;
  }

  private static bool IsEqual(object? a, object? b)
  {
    if (a is null && b is null) return true;
    if (a is null) return false;

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
