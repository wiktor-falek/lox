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
      if (Match(FUN))
      {
        if (!Check(IDENTIFIER))
        {
          Current--; // go back to FUN to match the lambda expression later
          return ExpressionStatement();
        }
        return Function("function");
      }
      if (Match(VAR)) return VarDeclaration();
      return Statement();
    }
    catch (ParseError)
    {
      Synchronize();
      return null;
    }
  }

  private FunctionStmt Function(string kind)
  {
    Token name = Consume(IDENTIFIER, $"Expect {kind} name.");
    Consume(LEFT_PAREN, $"Expect '( after {kind} name.");

    List<Token> parameters = [];

    if (!Check(RIGHT_PAREN))
    {
      do
      {
        if (parameters.Count >= 255)
        {
          Error(Peek(), "Can't have more than 255 parameters.");
        }
        parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
      } while (Match(COMMA));
    }

    Consume(RIGHT_PAREN, "Expect ')' after parameters.");

    Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");
    List<Stmt> body = Block();

    return new FunctionStmt(name, parameters, body);
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
    if (Match(RETURN)) return ReturnStatement();
    if (Match(LEFT_BRACE)) return new BlockStmt(Block());

    return ExpressionStatement();
  }

  private ReturnStmt ReturnStatement()
  {
    Token keyword = Previous();
    Expr? value = Check(SEMICOLON) ? null : Expression();
    Consume(SEMICOLON, "Expect ';' after return value.");
    return new ReturnStmt(keyword, value);
  }

  private BreakStmt BreakStatement()
  {
    Consume(SEMICOLON, "Expect ';' after break.");
    return new BreakStmt(Previous());
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
    Stmt thenBranch = Statement();

    List<IfStmt> elseIfStatements = [];
    Stmt? elseBranch = null;

    while (Match(ELSE))
    {
      if (Match(IF))
      {
        Consume(LEFT_PAREN, "Expect '(' after 'else if'.");
        Expr elseIfCondition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after 'else if' condition.");
        IfStmt elseIfStatement = new(elseIfCondition, Statement(), [], null);
        elseIfStatements.Add(elseIfStatement);
      }
      else
      {
        elseBranch = Statement();
        break;
      }
    }

    return new IfStmt(condition, thenBranch, elseIfStatements, elseBranch);
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
    Expr expr = Or();

    List<Expr> expressions = [expr];

    while (Match(COMMA))
    {
      Expr nextExpr = Or();
      expressions.Add(nextExpr);
    }

    if (expressions.Count > 1)
    {
      return new CommaExpr(expressions);
    }

    return expr;
  }

  private Expr Or()
  {
    Expr expr = And();

    if (Match(OR))
    {
      Token op = Previous();
      Expr right = And();
      expr = new LogicalExpr(expr, op, right);
    }

    return expr;
  }

  private Expr And()
  {
    Expr expr = Ternary();

    if (Match(AND))
    {
      Token op = Previous();
      Expr right = Ternary();
      expr = new LogicalExpr(expr, op, right);
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
      Expr first = Comma();
      if (first is CommaExpr comma)
      {
        arguments = comma.Expressions;
        if (arguments.Count >= 255)
        {
          Error(Peek(), "Can't have more than 255 arguments.)");
        }
      }
      else
      {
        arguments.Add(first);
      }
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

    if (Match(LEFT_PAREN))
    {
      Expr expr = Expression();
      Consume(RIGHT_PAREN, "Expect ')' after expression.");
      return new GroupingExpr(expr);
    }

    if (Match(IDENTIFIER)) return new VariableExpr(Previous());

    if (Match(FUN)) return Lambda();

    throw Error(Peek(), "Expect expression.");
  }

  private LambdaExpr Lambda()
  {
    Consume(LEFT_PAREN, $"Expect '( after lambda function.");

    List<Token> parameters = [];

    if (!Check(RIGHT_PAREN))
    {
      do
      {
        if (parameters.Count >= 255)
        {
          Error(Peek(), "Can't have more than 255 parameters.");
        }
        parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
      } while (Match(COMMA));
    }

    Consume(RIGHT_PAREN, "Expect ')' after parameters.");

    Consume(LEFT_BRACE, $"Expect '{{' before lambda body.");
    List<Stmt> body = Block();

    return new LambdaExpr(parameters, body);
  }
}
