using System;
using System.Collections.Generic;

namespace MonoGwent;

public class Statement {
    public List<string> keywords = new();
    public int Count{get => keywords.Count;}

    public void Add(string keyword) {
        keywords.Add(keyword);
    }
    public void Debug() {
        string debugList = string.Join(", ", keywords.ToArray());
        Console.WriteLine($"Statement keywords: {debugList}");
    }


    public object Clone() {
        Statement statement = new();

        foreach (var word in keywords) {
            statement.Add((string)word.Clone());
        }

        return statement;
    }
}