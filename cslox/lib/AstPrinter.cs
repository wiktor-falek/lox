using System.Text;

using R = System.String;

class AstPrinter : IExprVisitor<R>
{
  public void Print(Expr? expr)
  {
    if (expr is not null)
    {
      Console.WriteLine(expr.Accept(this));
    }
  }

  R IExprVisitor<R>.VisitAssignExpr(AssignExpr expr)
  {
    throw new NotImplementedException();
  }

  R IExprVisitor<R>.VisitBinaryExpr(BinaryExpr expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
  }

  R IExprVisitor<R>.VisitGroupingExpr(GroupingExpr expr)
  {
    return Parenthesize("group", expr.Expression);
  }

  R IExprVisitor<R>.VisitLiteralExpr(LiteralExpr expr)
  {
    if (expr.Value == null)
    {
      return "nil";
    }
    else
    {
      return expr.Value.ToString() ?? "nil";
    }
  }

  R IExprVisitor<R>.VisitUnaryExpr(UnaryExpr expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Right);
  }

  R IExprVisitor<R>.VisitTernaryExpr(TernaryExpr expr)
  {
    return Parenthesize("ternary", expr.Condition, expr.TrueExpr, expr.FalseExpr);
  }

  R IExprVisitor<R>.VisitCommaExpr(CommaExpr expr)
  {
    return Parenthesize("comma", [.. expr.Expressions]);
  }

  public string VisitVariableExpr(VariableExpr expr)
  {
    throw new NotImplementedException();
  }

  private string Parenthesize(string name, params Expr[] exprs)
  {
    StringBuilder builder = new();

    builder.Append('(').Append(name);
    foreach (Expr expr in exprs)
    {
      builder.Append(' ');
      builder.Append(expr.Accept(this));
    }
    builder.Append(')');

    return builder.ToString();
  }

  public string VisitCallExpr(CallExpr expr)
  {
    throw new NotImplementedException();
  }

  public string VisitLambdaExpr(LambdaExpr expr)
  {
    throw new NotImplementedException();
  }
}
