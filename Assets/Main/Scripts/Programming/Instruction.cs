using System.Collections.Generic;
using UnityEngine;
using System;


public interface IExecutable
{
    void Execute(IConsoleToMap iMap);
    string Name();
}

public abstract class Instruction : IExecutable
{
    protected Queue<IInstructionParam> parameters; // red parametara u "Param" formatu
    protected Queue<string> paramsStr; // red parametara u string formatu
    protected int paramCount = -1; // -1 == beskonacno

    public Instruction(string par, int parCount)
    {
        parameters = new Queue<IInstructionParam>();
        paramsStr = new Queue<string>();
        paramCount = parCount;

        // Odbacivanje praznih elemenata nastalih zbog vise uzastopnih blanko karaktera
        string[] parStr = par.Split(' ');
        foreach (var p in parStr)
        {
            string tmp = p.Trim();
            if (tmp != "")
                paramsStr.Enqueue(tmp);
        }

        // Check 1. - Param Count
        if(paramsStr.Count != paramCount) {
            Debug.LogError("Instruction Construction Error!");
            throw new InsParException();
        }
    }

    public abstract void Execute(IConsoleToMap iMap);
    public abstract string Name();

    public override string ToString()
    {
        string p = "";
        foreach (string item in paramsStr.ToArray()) {
            Debug.Log("> " + item);
            p = p + " " + item;
        }
        return Name() + ":" + p;
    }
}


// > Exceptions

public class InstructionException : Exception {}
public class InsParException : InstructionException {}