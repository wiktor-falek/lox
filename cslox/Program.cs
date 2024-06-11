class Lox
{
  public static bool hadError = false;

  static void Main(string[] args)
  {
    if (args.Length > 1)
    {
      Console.WriteLine("Usage: jlox [script]");
      Environment.Exit(64);
    }
    else if (args.Length == 1)
    {
      RunFile(args[0]);
    }
    else
    {
      RunPrompt();
    }
    Environment.Exit(0);
  }

  static void Run(string source)
  {
    Scanner scanner = new(source);
    List<Token> tokens = scanner.ScanTokens();
    foreach (var token in tokens)
    {
      Console.WriteLine(token);
    }
  }

  static void RunFile(string fileName)
  {
    try
    {
      StreamReader sr = new(Path.GetFullPath(fileName));
      string content = sr.ReadToEnd();
      Run(content);
      if (hadError) Environment.Exit(65);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }

  static void RunPrompt()
  {
    while (true)
    {
      Console.Write("> ");
      string? line = Console.ReadLine();
      if (line == null) break;
      Run(line);
      hadError = false;
    }
  }

  public static void Error(int line, string message)
  {
    Report(line, "", message);
  }

  public static void Report(int line, string where, string message)
  {
    Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
    hadError = true;
  }
}
