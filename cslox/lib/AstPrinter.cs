using System.Text;

using R = System.String;

class AstPrinter : IVisitor<R>
{
  public void Print(Expr expr)
  {
    Console.WriteLine(expr.Accept(this));
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