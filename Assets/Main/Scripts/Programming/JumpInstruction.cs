using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// move: (N,S,E,W) [1,]
public class JumpInstruction : Instruction
{
    public JumpInstruction(string par) : base(par, 0) { }

    public override void Execute(IConsoleToMap iMap)
    {
        iMap.PlayerJump();
    }

    public override string Name() => "jump";
}