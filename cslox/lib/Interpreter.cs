using static TokenType;

public class RuntimeError(Token token, string message) : SystemException(message)
{
  public readonly Token Token = token;
}

public class Interpreter : IVisitor<object?>
{
  public void Interpret(Expr expression)
  {
    try
    {
      object? value = Evaluate(expression);
      Console.WriteLine(Stringify(value));
    }
    catch (RuntimeError error)
    {
      Lox.RuntimeError(error);
    }
  }

  object? IVisitor<object?>.VisitLiteralExpr(Literal expr)
  {
    return expr.Value;
  }

  object? IVisitor<object?>.VisitGroupingExpr(Grouping expr)
  {
    return Evaluate(expr.Expression);
  }

  object? IVisitor<object?>.VisitUnaryExpr(Unary expr)
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

  object? IVisitor<object?>.VisitBinaryExpr(Binary expr)
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
        };
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

  object? IVisitor<object?>.VisitTernaryExpr(Ternary expr)
  {
    if (IsTruthy(Evaluate(expr.Condition)))
    {
      return Evaluate(expr.TrueExpr);
    }
    return Evaluate(expr.FalseExpr);
  }

  object? IVisitor<object?>.VisitCommaExpr(Comma expr)
  {
    object? lastExprValue = null;

    foreach (Expr expression in expr.Expressions)
    {
      lastExprValue = Evaluate(expression);
    }

    return lastExprValue;
  }

  private static string Stringify(object? obj)
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
    if (obj is bool v) return v;
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
