using System.Text;

using R = System.String;

class AstPrinter : IVisitor<R>
{
  public void Print(Expr? expr)
  {
    if (expr is not null)
    {
      Console.WriteLine(expr.Accept(this));
    }
  }

  R IVisitor<R>.VisitBinaryExpr(Binary expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
  }

  R IVisitor<R>.VisitGroupingExpr(Grouping expr)
  {
    return Parenthesize("group", expr.Expression);
  }

  R IVisitor<R>.VisitLiteralExpr(Literal expr)
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

  R IVisitor<R>.VisitUnaryExpr(Unary expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Right);
  }

  R IVisitor<R>.VisitTernaryExpr(Ternary expr)
  {
    return Parenthesize("ternary", expr.Condition, expr.TrueExpr, expr.FalseExpr);
  }

  R IVisitor<R>.VisitCommaExpr(Comma expr)
  {
    return Parenthesize("comma", [.. expr.Expressions]);
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
}