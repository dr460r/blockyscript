using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NaturalNumParam : InstructionParam<int>
{
    public NaturalNumParam(string s) : base(s)
    {
        value = -1;

        if(!TryParse(s, ref value))
            throw new InsParException();

        // Debug.Log("Natural Number Param: Constructed!");
    }

    public override bool TryParse(string s, ref int v)
    {
        if(!CanParse(s))
            return false;

        // Parse
        v = int.Parse(s);
        return true;
    }

    public override bool CanParse(string p)
    {
        int val;
        return int.TryParse(p, out val) && val >= 1;
    }
}