

using System.IO;

namespace MonoGwent;

class ScriptReader {
    private static string script_path = Path.Join(
        "Content",
        "script.txt"
    );

    private static string Read() {
        if (Path.Exists(script_path))
            return File.ReadAllText(script_path);
        return "";
    }

    public static void Initialize() {
        var text = Read();
        if (text.Length == 0) return;

        var lexer = new Lexer(text);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens);
        parser.Parse();
    }
}