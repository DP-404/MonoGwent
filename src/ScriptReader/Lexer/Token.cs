
namespace MonoGwent;

public class Token {
    public TokenType type { get; }
    public string val { get; }
    public int pos { get; }

    public Token(TokenType type, string val, int pos) {
        this.type = type;
        this.val = val;
        this.pos = pos;
    }

}