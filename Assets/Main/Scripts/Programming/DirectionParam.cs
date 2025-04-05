using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DirectionParam : InstructionParam<Vector2Int>
{
    protected Dictionary<string, Vector2Int> mapping;

    public DirectionParam(string s) : base(s)
    {
        mapping = new Dictionary<string, Vector2Int>();
        Vector2Int N = new Vector2Int(0,-1);
        Vector2Int S = new Vector2Int(0,1);
        Vector2Int E = new Vector2Int(-1,0);
        Vector2Int W = new Vector2Int(1,0);

        mapping.Add("N", N);
        mapping.Add("E", E);
        mapping.Add("S", S);
        mapping.Add("W", W);

        if(!TryParse(s, ref value))
            throw new InsParException();

        // Debug.Log("DirectionParam: Constructed!");
    }

    public override bool TryParse(string s, ref Vector2Int v)
    {
        if(!CanParse(s)) 
            return false;
        
        // Parse
        v = mapping[s];
        return true;
    }

    public override bool CanParse(string p)
    {
        foreach (var k in mapping.Keys)
            if(p == k) return true;

        return false;
    }
}