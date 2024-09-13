using System;
using System.IO;

namespace MonoGwent;

class ScriptReader {
    public static readonly string ERROR_PATH = "scripterror.txt";
    public static readonly string SCRIPT_PATH = Path.Join(
        "Content",
        "script.txt"
    );
    private static bool _error = false;
    public static bool error {get => _error;}

    private static string Read() {
        if (Path.Exists(SCRIPT_PATH))
            return File.ReadAllText(SCRIPT_PATH);
        else {
            File.Create(SCRIPT_PATH);
            return "";
        }
    }

    public static void Initialize() {
        var text = Read();
        if (text.Length == 0) return;

        var lexer = new Lexer(text);

        try {
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            parser.Parse();
        }
        catch (Exception e) {
            _error = true;
            using (var writer = new StreamWriter(ERROR_PATH)) {
                writer.WriteLine(e);
            }
        }
    }
}