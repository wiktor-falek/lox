class GenerateAst
{
  static void Main(string[] args)
  {
    if (args.Length != 1)
    {
      Console.WriteLine("Usage: generate_ast <output directory>");
      Environment.Exit(64);
    }

    string outputDir = args[0];
    DefineAst(outputDir, "Expr", "object?", [
      "AssignExpr   : Token name, Expr value",
      "BinaryExpr   : Expr left, Token op, Expr right",
      "GroupingExpr : Expr expression",
      "LiteralExpr  : object? value",
      "UnaryExpr    : Token op, Expr right",
      "TernaryExpr  : Expr condition, Expr trueExpr, Expr falseExpr",
      "CommaExpr    : List<Expr> expressions",
      "VariableExpr : Token name",
    ]);

    DefineAst(outputDir, "Stmt", "void", [
      "IfStmt    : Expr condition, Stmt thenBranch, Stmt? elseBranch",
      "BlockStmt : List<Stmt> statements",
      "ExprStmt  : Expr expression",
      "PrintStmt : Expr expression",
      "VarStmt   : Token name, Expr? initializer",
    ]);
  }

  private static void DefineAst(string outputDir, string baseName, string visitorReturnType, List<string> types)
  {
    string path = Path.Combine(outputDir, $"{baseName}.cs");
    using StreamWriter writer = new(path);

    if (visitorReturnType == "void")
    {
      DefineVoidReturnVisitor(writer, baseName, types);
    }
    else
    {
      DefineVisitor(writer, baseName, types);
    }

    writer.WriteLine($"abstract public class {baseName}");
    writer.WriteLine("{");
    if (visitorReturnType == "void")
    {
      writer.WriteLine($"  abstract public void Accept(I{baseName}Visitor visitor);");
    }
    else
    {
      writer.WriteLine($"  abstract public R Accept<R>(I{baseName}Visitor<R> visitor);");

    }
    writer.WriteLine("}");

    foreach (string type in types)
    {

      string className = type.Split(":")[0].Trim();
      string fields = type.Split(":")[1].Trim();
      DefineType(writer, baseName, className, visitorReturnType, fields);
    }
  }

  private static void DefineType(StreamWriter writer, string baseName, string className, string visitorReturnType, string fieldList)
  {
    IEnumerable<string> fields = fieldList.Split(",").Select(e => e.Trim());

    writer.WriteLine($"\npublic class {className} : {baseName}");
    writer.WriteLine("{");
    foreach (string field in fields)
    {
      string fieldType = field.Split(" ")[0];
      string fieldName = field.Split(" ")[1];
      string capitalizedFieldName = fieldName[..1].ToUpper() + fieldName[1..];
      writer.WriteLine($"  public readonly {fieldType} {capitalizedFieldName};");
    }
    writer.Write($"  public {className}(");

    string last = fields.Last();
    foreach (string field in fields)
    {
      string fieldType = field.Split(" ")[0];
      string fieldName = field.Split(" ")[1];

      if (field != last)
      {
        writer.Write($"{fieldType} {fieldName}, ");
      }
      else
      {
        writer.Write($"{fieldType} {fieldName})");
      }
    }
    writer.WriteLine("  {");
    foreach (string field in fields)
    {
      string fieldName = field.Split(" ")[1];
      string capitalizedFieldName = fieldName[..1].ToUpper() + fieldName[1..];
      writer.WriteLine($"    {capitalizedFieldName} = {fieldName};");
    }
    writer.WriteLine("  }");

    if (visitorReturnType == "void")
    {
      writer.WriteLine($"  override public void Accept(I{baseName}Visitor visitor)");
      writer.WriteLine("  {");
      writer.WriteLine($"    visitor.Visit{className}(this);");
      writer.WriteLine("  }");
    }
    else
    {
      writer.WriteLine($"  override public R Accept<R>(I{baseName}Visitor<R> visitor)");
      writer.WriteLine("  {");
      writer.WriteLine($"    return visitor.Visit{className}(this);");
      writer.WriteLine("  }");
    }
    writer.WriteLine("}");
  }

  private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
  {
    writer.WriteLine($"public interface I{baseName}Visitor<R>");
    writer.WriteLine("{");
    foreach (string type in types)
    {
      string className = type.Split(":")[0].Trim();
      writer.WriteLine($"  R Visit{className}({className} {baseName.ToLower()});");
    }
    writer.WriteLine("}\n");
  }

  private static void DefineVoidReturnVisitor(StreamWriter writer, string baseName, List<string> types)
  {
    writer.WriteLine($"public interface I{baseName}Visitor");
    writer.WriteLine("{");
    foreach (string type in types)
    {
      string className = type.Split(":")[0].Trim();
      writer.WriteLine($"  void Visit{className}({className} {baseName.ToLower()});");
    }
    writer.WriteLine("}\n");
  }
}
