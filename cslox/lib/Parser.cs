
using static TokenType;

class Parser
{
  private readonly List<Token> Tokens;
  private int Current = 0;

  Parser(List<Token> tokens)
  {
    Tokens = tokens;
  }

  private bool Match(params TokenType[] types)
  {
    bool matched = types.Any(Check);

    if (matched)
    {
      Advance();
    }

    return matched;
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd()) return false;
    return Peek().Type == type;
  }

  private Token Advance()
  {
    if (!IsAtEnd()) Current++;
    return Previous();
  }

  private bool IsAtEnd()
  {
    return Peek().Type == EOF;
  }

  private Token Peek()
  {
    return Tokens.ElementAt(Current);
  }

  private Token Previous()
  {
    return Tokens.ElementAt(Current - 1);
  }

  private Token Consume(TokenType type, string message)
  {
    throw new NotImplementedException();
  }

  private Expr Expression()
  {
    return Equality();
  }

  private Expr Equality()
  {
    Expr expr = Comparison();

    while (Match(BANG_EQUAL, EQUAL_EQUAL))
    {
      Token op = Previous();
      Expr right = Comparison();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }

  private Expr Comparison()
  {
    Expr expr = Term();

    while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
    {
      Token op = Previous();
      Expr right = Term();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }

  private Expr Term()
  {
    Expr expr = Factor();

    while (Match(MINUS, PLUS))
    {
      Token op = Previous();
      Expr right = Factor();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }

  private Expr Factor()
  {
    Expr expr = Unary();

    while (Match(SLASH, STAR))
    {
      Token op = Previous();
      Expr right = Unary();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }

  private Expr Unary()
  {
    if (Match(BANG, MINUS))
    {
      Token op = Previous();
      Expr right = Primary();
      return new Unary(op, right);
    }

    return Primary();
  }

  private Expr Primary()
  {
    // grouping
    if (Match(TRUE)) return new Literal(true);
    if (Match(FALSE)) return new Literal(false);
    if (Match(NIL)) return new Literal(null);

    if (Match(NUMBER, STRING))
    {
      return new Literal(Previous().Literal);
    }

    if (Match(LEFT_PAREN))
    {
      Expr expr = Expression();
      Consume(RIGHT_PAREN, "Expect ')' after expression.");
      return new Grouping(expr);
    }

    throw new Exception("TODO");
  }
}
