using System.Collections.Generic;

// s - Text Instrukcije / Ime Procedure
// l - null             / Lista IExecutable za Proceduru
delegate IExecutable ReturnIExecutable(string s, Queue<IExecutable> l);

class InstructionNameMapping : Dictionary<string, ReturnIExecutable>
{
    public void Add(string[] keys, ReturnIExecutable inst)
    {
        foreach (string k in keys)
        {
            base.Add(k, inst);
        }
    }
}
