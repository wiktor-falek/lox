using static TokenType;

public class ParseError : SystemException { }

public class Parser(List<Token> tokens)
{
  private readonly List<Token> Tokens = tokens;
  private int Current = 0;

  public List<Stmt> Parse()
  {
    List<Stmt> statements = [];

    while (!IsAtEnd())
    {
      Stmt? statement = Declaration();
      if (statement is not null)
      {
        statements.Add(statement);
      }
    }

    return statements;
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
    if (Check(type)) return Advance();

    throw Error(Peek(), message);
  }

  private static ParseError Error(Token token, string message)
  {
    Lox.Error(token, message);
    return new ParseError();
  }

  private void Synchronize()
  {
    Advance();

    while (!IsAtEnd())
    {
      if (Previous().Type == SEMICOLON) return;

      switch (Peek().Type)
      {
        case CLASS:
        case FUN:
        case VAR:
        case FOR:
        case IF:
        case WHILE:
        case PRINT:
        case RETURN:
          return;
      }
    }

    Advance();
  }

  private Stmt? Declaration()
  {
    try
    {
      if (Match(VAR)) return VarDeclaration();
      return Statement();
    }
    catch (ParseError)
    {
      Synchronize();
      return null;
    }
  }

  private VarStmt VarDeclaration()
  {
    Token name = Consume(IDENTIFIER, "Expect variable name.");
    Expr? initializer = Match(EQUAL) ? Expression() : null;
    Consume(SEMICOLON, "Expect ';' after variable declaration.");

    return new VarStmt(name, initializer);
  }

  private Stmt Statement()
  {
    if (Match(IF)) return IfStatement();
    if (Match(FOR)) return ForStatement();
    if (Match(WHILE)) return WhileStatement();
    if (Match(BREAK)) return BreakStatement();
    if (Match(PRINT)) return PrintStatement();
    if (Match(LEFT_BRACE)) return new BlockStmt(Block());

    return ExpressionStatement();
  }

  private BreakStmt BreakStatement()
  {
    Consume(SEMICOLON, "Expect ';' after break.");
    return new BreakStmt(Previous());
  }

  private PrintStmt PrintStatement()
  {
    Expr value = Expression();
    Consume(SEMICOLON, "Expect ';' after value.");

    return new PrintStmt(value);
  }

  private List<Stmt> Block()
  {
    List<Stmt> statements = [];

    while (!Check(RIGHT_BRACE) && !IsAtEnd())
    {
      var statement = Declaration();
      if (statement is not null)
      {
        statements.Add(statement);
      }
    }

    Consume(RIGHT_BRACE, "Expect '}' after block.");
    return statements;
  }

  private IfStmt IfStatement()
  {
    Consume(LEFT_PAREN, "Expect '(' after 'if'.");
    Expr condition = Expression();
    Consume(RIGHT_PAREN, "Expect ')' after if condition.");

    Stmt thenCondition = Statement();
    Stmt? elseCondition = Match(ELSE) ? Statement() : null;

    return new IfStmt(condition, thenCondition, elseCondition);
  }

  private Stmt ForStatement()
  {
    Consume(LEFT_PAREN, "Expect '(' after 'for'.");

    Stmt? initializer;
    if (Match(SEMICOLON))
    {
      initializer = null;
    }
    else if (Match(VAR))
    {
      initializer = VarDeclaration();
    }
    else
    {
      initializer = ExpressionStatement();
    }

    Expr condition = Check(SEMICOLON) ? new LiteralExpr(true) : Expression();
    Consume(SEMICOLON, "Expect ';' after loop condition.");
    Expr? increment = Check(RIGHT_PAREN) ? null : Expression();
    Consume(RIGHT_PAREN, "Expect ')' after for clauses.");
    Stmt body = Statement();

    if (increment is not null)
    {
      body = new BlockStmt([body, new ExprStmt(increment)]);
    }

    body = new WhileStmt(condition, body);

    if (initializer is not null)
    {
      body = new BlockStmt([initializer, body]);
    }

    return body;
  }

  private WhileStmt WhileStatement()
  {
    Consume(LEFT_PAREN, "Expect '(' after 'while'.");
    Expr condition = Expression();
    Consume(RIGHT_PAREN, "Expect ')' after while condition.");

    Stmt body = Statement();

    return new WhileStmt(condition, body);
  }

  private ExprStmt ExpressionStatement()
  {
    Expr expr = Expression();
    Consume(SEMICOLON, "Expect ';' after expression.");

    return new ExprStmt(expr);
  }

  private Expr Expression()
  {
    return Assignment();
  }

  private Expr Assignment()
  {
    Expr expr = Comma();

    if (Match(EQUAL))
    {
      Token equals = Previous();
      Expr value = Assignment();

      if (expr is VariableExpr variableExpr)
      {
        Token name = variableExpr.Name;
        return new AssignExpr(name, value);
      }

      Error(equals, "Invalid assignment target.");
    }

    return expr;
  }

  private Expr Comma()
  {
    Expr expr = Ternary();

    List<Expr> expressions = [expr];

    while (Match(COMMA))
    {
      Expr nextExpr = Ternary();
      expressions.Add(nextExpr);
    }

    if (expressions.Count > 1)
    {
      return new CommaExpr(expressions);
    }

    return expr;
  }

  private Expr Ternary()
  {
    Expr expr = Equality();

    if (Match(QUESTION_MARK))
    {
      Expr left = Expression();

      Consume(COLON, "Expect ':' in ternary.");

      Expr right = Ternary();

      return new TernaryExpr(expr, left, right);
    }

    return expr;
  }

  private Expr Equality()
  {
    Expr expr = Comparison();

    while (Match(BANG_EQUAL, EQUAL_EQUAL))
    {
      Token op = Previous();
      Expr right = Comparison();
      expr = new BinaryExpr(expr, op, right);
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
      expr = new BinaryExpr(expr, op, right);
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
      expr = new BinaryExpr(expr, op, right);
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
      expr = new BinaryExpr(expr, op, right);
    }

    return expr;
  }

  private Expr Unary()
  {
    if (Match(BANG, MINUS))
    {
      Token op = Previous();
      Expr right = Primary();
      return new UnaryExpr(op, right);
    }

    return Call();
  }

  private Expr Call()
  {
    Expr expr = Primary();

    while (true)
    {
      if (Match(LEFT_PAREN))
      {
        expr = FinishCall(expr);
      }
      else
      {
        break;
      }
    }

    return expr;
  }

  private CallExpr FinishCall(Expr callee)
  {
    List<Expr> arguments = [];
    if (!Check(RIGHT_PAREN))
    {
      do
      {
        if (arguments.Count >= 255)
        {
          Error(Peek(), "Can't have more than 255 arguments.)");
        }
        arguments.Add(Expression());
      } while (Match(COMMA));
    }

    Token paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

    return new CallExpr(callee, paren, arguments);
  }

  private Expr Primary()
  {
    if (Match(TRUE)) return new LiteralExpr(true);
    if (Match(FALSE)) return new LiteralExpr(false);
    if (Match(NIL)) return new LiteralExpr(null);

    if (Match(NUMBER, STRING))
    {
      return new LiteralExpr(Previous().Literal);
    }

    if (Match(IDENTIFIER)) return new VariableExpr(Previous());

    if (Match(LEFT_PAREN))
    {
      Expr expr = Expression();
      Consume(RIGHT_PAREN, "Expect ')' after expression.");
      return new GroupingExpr(expr);
    }

    throw Error(Peek(), "Expect expression.");
  }
}
