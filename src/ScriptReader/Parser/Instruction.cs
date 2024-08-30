using System;
using System.Collections.Generic;

namespace MonoGwent;

public class Instruction
{

    public List<string> Keywords = new();


    public void Add(string keyword)
    {
        Keywords.Add(keyword);
    }

    public int Count{get{return Keywords.Count;}}

    public void Debug()
    {
        string debugList = string.Join(", ", Keywords.ToArray());
        Console.WriteLine($"Instruction keywords: {debugList}");
    }


    public object Clone()
    {
        Instruction instruction = new();

        foreach(string word in Keywords)
        {
            instruction.Add((string)word.Clone());
        }

        return instruction;
    }
}