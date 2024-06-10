void run(string source) {
  // Scanner scanner = new(source);
  // List<Token> tokens = scanner.ScanTokens();
  // foreach (var token in tokens)
  // {
  //   Console.WriteLine(token);
  // }
}

void runFile(string fileName) {
  try {
  StreamReader sr = new(Path.GetFullPath(fileName));
  string content = sr.ReadToEnd();
  run(content);
  } catch(Exception e) {
    Console.WriteLine(e);
  }
}

void runPrompt() {
  while (true) {
    Console.Write("> ");
    string? line = Console.ReadLine();
    if (line == null) break;
    run(line);
  }
  
}

if (args.Length > 1) {
  Console.WriteLine("Usage: jlox [script]");
  return 64;
} else if (args.Length == 1)
{
  runFile(args[0]);
} else {
  runPrompt();
}

return 0;