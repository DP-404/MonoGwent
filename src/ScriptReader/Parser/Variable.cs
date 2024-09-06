using System;

namespace MonoGwent;

public class Variable : ICloneable {
    public string name;
    public object val;
    public string type;

    public Variable(string name, string type) {
        this.name = name;
        this.type = type;
    }

    public object Clone() {
        return new Variable(name, type) {val = val};
    }
}