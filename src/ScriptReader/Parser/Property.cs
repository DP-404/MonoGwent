
namespace MonoGwent;

public class Property {
    public readonly string type;
    public readonly object value;

    public Property(string type, object value) {
        this.type = type;
        this.value = value;
    }
}