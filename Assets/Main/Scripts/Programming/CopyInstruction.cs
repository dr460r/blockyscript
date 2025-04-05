using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// pick: (N,S,E,W)
public class CopyInstruction : Instruction
{
    public CopyInstruction(string par) : base(par, 1)
    {
        // Check 2. - Uncompatabile Types (Automatic Check)
        // BITAN JE REDOSLED NAVODJENA PARAMETARA ISPOD (zbog Dequeue)
        parameters.Enqueue(new DirectionParam(paramsStr.Dequeue()));
    }

    public override void Execute(IConsoleToMap iMap)
    {
        Vector2Int dir = (parameters.Dequeue() as DirectionParam).Value;
        
        iMap.PlayerCopyItem(dir);
    }

    public override string Name() => "copy";
}