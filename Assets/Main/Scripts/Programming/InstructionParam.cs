using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInstructionParam
{
    bool CanParse(string p);
}

public abstract class InstructionParam<ValT> : IInstructionParam
{
    protected ValT value;
    public ValT Value { get => value; }

    public InstructionParam(string s) {}


    // > Abstract
    public abstract bool TryParse(string s, ref ValT v);
    // From Interface
    public abstract bool CanParse(string p);
}
