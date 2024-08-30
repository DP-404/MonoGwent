
namespace MonoGwent;

public class Property {
    public readonly string type;
    public readonly string value;
    public readonly int val_int;

    public Property(string type, string value)
    {
        this.type = type;
        this.value = value;
    }

    public Property(string type, int value)
    {
        this.type = type;
        this.val_int = value;
    }
}