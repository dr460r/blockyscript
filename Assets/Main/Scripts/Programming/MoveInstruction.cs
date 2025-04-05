using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// move: (N,S,E,W) [1,]
public class MoveInstruction : Instruction
{
    public MoveInstruction(string par) : base(par, 2)
    {   
        try
        {
            // Check 2. - Uncompatabile Types (Automatic Check)
            // BITAN JE REDOSLED NAVODJENA PARAMETARA ISPOD (zbog Dequeue)
            parameters.Enqueue(new DirectionParam(paramsStr.Dequeue()));
            parameters.Enqueue(new NaturalNumParam(paramsStr.Dequeue()));
        }
        catch (InstructionException)
        {
            Debug.LogError("Move Instruction Construction Error!");
            throw;
        }
    }

    public override void Execute(IConsoleToMap iMap)
    {
        Vector2Int dir = (parameters.Dequeue() as DirectionParam).Value;
        int dist = (parameters.Dequeue() as NaturalNumParam).Value;

        Debug.Log("Execute Move: " + (dir*dist));
        iMap.PlayerMove(dir * dist);
    }

    public override string Name() => "move";
}