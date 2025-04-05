using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class ConsoleController : MonoBehaviour
{
    // Static
    //private static ConsoleController instance;
    public static ConsoleController Instance { get; set; }

    public event VoidFunc EventInstructionFinished;
    public event VoidFunc EventInstructionInterrupted;

    public static void InstructionFinished() 
    {
        if (Instance.callStack.Count == 0)
            return;
        Instance.EventInstructionFinished?.Invoke();
    }
    public static void InstructionInterrupted() 
    {
        if (Instance.callStack.Count == 0)
            return;
        Instance.EventInstructionInterrupted?.Invoke();
    }
        


    public static void SubscribeBlock(InstructionBlock block) 
    {
        // Debug.LogAssertion("Subscribe");
        Instance.EventInstructionFinished += block.NextInstruction;
        Instance.EventInstructionInterrupted += block.StopExecution;

        Instance.callStack.Push(block);
    }

    public static void UnsubscribeBlock(InstructionBlock block) 
    {
        // Debug.LogAssertion("Unsubscribe");
        Instance.EventInstructionFinished -= block.NextInstruction;
        Instance.EventInstructionInterrupted -= block.StopExecution;

        InstructionBlock poped = null;
        if (Instance.callStack.Count > 0) { // callStack je prazan nakon sto se izvrsi ResetConsole pozivom InstructionInterrupted metode
            poped = Instance.callStack.Pop();
        }

        if (Instance.EventInstructionFinished != null)    
            InstructionFinished();

        if(Instance.callStack.Count == 0) {
            Instance.ResetConsole();
        }
    }

    public static bool IsBlockOnStackTop(InstructionBlock block)
    {
        return Instance.callStack.Peek() == block;
    }


    // Settings
    public string mainMethodName;

    // UI
    public TMP_InputField inpMainCode;
    public Button btnRun;

    // Game References
    public MapController map;

    // Instructions
    InstructionNameMapping instructionNameMapping;
    int branchDepth = 10;
    // InstructionNameMapping procedureNameMapping;
    
    // Local State
    string codeText;
    public List<IExecutable> procedures;
    Stack<InstructionBlock> callStack = new Stack<InstructionBlock>();
    // Dictionary<string, List<IExecutable>> procedureNameMapping;

    /* ===== Game ===== */

    void Awake()
    {
        // Set Singleton
        if(ConsoleController.Instance != null)
            GameObject.Destroy(ConsoleController.Instance);
        ConsoleController.Instance = this;
    }

    void Start()
    {
        // > Instruction Interruption Callback
        EventInstructionInterrupted += () => { 
            Debug.LogWarning(callStack.Peek().InsCurrent.ToString());
            ResetConsole(); 
        };

        // > Mappings
        InstructionNameMapping im = new InstructionNameMapping();
        
        im.Add(new string[] {"move", "m"}, (s,l)=> new MoveInstruction(s));
        im.Add(new string[] {"pick", "p"}, (s,l)=> new PickInstruction(s));
        im.Add(new string[] {"copy", "c"}, (s,l)=> new CopyInstruction(s));
        im.Add(new string[] {"drop", "d"}, (s,l)=> new DropInstruction(s));
        im.Add(new string[] {"jump", "j"}, (s,l)=> new JumpInstruction(s));

        instructionNameMapping = im;
    }

    /* ===== Methods ====== */

    public void ResetMap()
    {
        map.Regenerate();
        ResetConsole();
    }

    public void ResetConsole()
    {
        (btnRun as Button).interactable = true; // BTN Run
        procedures = null;
        codeText = "";
        callStack = new Stack<InstructionBlock>();
        Debug.LogWarning("Console Reset");
    }

    public void RunCode()
    {
        (btnRun as Button).interactable = false; // BTN Run

        codeText = inpMainCode.text;
        string[] procStr = codeText.TrimStart('@').Split('@');

        // TESTING!!! Repeat Command
        //ProcessRepeatKeyword(procStr);

        procedures = new List<IExecutable>();

        foreach (string proc in procStr)
        {
            try
            {
                procedures.Add(CreateProcedure(proc));
            }
            catch (InstructionException)
            {
                // Debug.LogError("InstructionException");
                throw;
            }
        }

        foreach (var proc in procedures)
        {
            if (proc.Name() == mainMethodName) {
                proc.Execute(map);
                break;
            }
        }

        // foreach (string proc in procStr)
        // {
        //     try
        //     {
        //         CreateProcedure(proc);
        //     }
        //     catch (InstructionException)
        //     {
        //         // Debug.LogError("InstructionException");
        //         return;
        //     }
        // }

        // instructionNameMapping["Main"]("",null).Execute(map);
    }

    IExecutable CreateProcedure(string codeText, int branchDepth = 0)
    {
        string[] codeLines = codeText.Split('\n');
        Queue<IExecutable> instructions = new Queue<IExecutable>();

        string procName = codeLines[0].Trim();
        
        // // Debug.LogWarning("New procedure name: " + procName);

        for (int i = 1; i < codeLines.Length; i++)
        {
            string line = codeLines[i];
            try
            {
                if (line.Trim() == "") continue;

                string[] ins = line.Trim().Split(':');
                string insName = ins[0].Trim();
                bool isIns = false;
                foreach (var k in instructionNameMapping.Keys)
                {
                    if (k == insName) {
                        isIns = true;
                        break;
                    }
                }

                // // Debug.LogWarning(ins.Length);

                // TESTING!!! Repeat Command --------------------------
                Debug.Log("Linija, pre !, je: " + line);
                char repChar = '!';
                if (!isIns && line.Trim().Split(' ')[0].Trim()[0] == repChar) 
                {
                    Debug.Log("Prepoznat je ! deo");

                    string repNumStr = line.Trim().Split(' ')[0].Trim().Split(repChar)[1];
                    int repNum = 0;

                    try {
                        repNum = int.Parse(repNumStr);
                    } 
                    catch (Exception) {
                        Debug.LogError("Invalid Repeat statement!");
                        throw;
                    }

                    char[] chrarrtmp = new char[1]; chrarrtmp[0] = ' '; // ovo je mnogo lose...
                    string[] ins2 = line.Trim().Split(chrarrtmp, 2)[1].Trim().Split(':');
                    string insName2 = ins2[0].Trim();
                    bool isIns2 = false;
                    foreach (var k in instructionNameMapping.Keys)
                    {
                        if (k == insName2) {
                            isIns2 = true;
                            break;
                        }
                    }

                    //Debug.Log("Ins: " + ins2[0] + " " + ins2[1]);

                    if (isIns2) 
                    {
                        string insParams = "";
                        if (ins2.Length == 2)
                            insParams = ins2[1].Trim();

                        Debug.Log("Ins Name: " + insName2 + ", Ins Params: " + insParams);
                        
                        for (int ji = 0; ji < repNum; ji++)
                            instructions.Enqueue(instructionNameMapping[insName2](insParams, null));
                    }
                    else {
                        if (branchDepth < this.branchDepth)
                        {
                            foreach (var proc in procedures)
                            {
                                if (proc.Name() == insName2) {
                                    // // Debug.Log("Procedure "+insName+" added to precedure "+procName+"!");
                                    for (int ji = 0; ji < repNum; ji++)
                                        instructions.Enqueue(CreateProcedure((proc as InstructionBlock).codeText, branchDepth + 1));
                                    break;
                                }
                            }
                        }
                    }

                    continue;
                }
                // -------------------------------


                if (isIns) 
                {
                    string insParams = "";
                    if (ins.Length == 2)
                        insParams = ins[1].Trim();
                    instructions.Enqueue(instructionNameMapping[insName](insParams, null));
                }
                else {
                    if (branchDepth < this.branchDepth)
                    {
                        foreach (var proc in procedures)
                        {
                            if (proc.Name() == insName) {
                                // // Debug.Log("Procedure "+insName+" added to precedure "+procName+"!");

                                instructions.Enqueue(CreateProcedure((proc as InstructionBlock).codeText, branchDepth + 1));
                                break;
                            }
                        }
                    }
                }
            }
            catch (InstructionException)
            {
                // Debug.LogError("Create Procedure Failed!");
                throw;
            }
        }

        // instructionNameMapping.Add(new string[] {procName}, (s,l) => new InstructionBlock(s, l));
        // return instructionNameMapping[procName](procName, instructions);

        InstructionBlock bl = new InstructionBlock(procName, instructions);
        bl.codeText = codeText;
        return bl;
    }

    // TESTING!!! Repeat Command
    private void ProcessRepeatKeyword(string[] procText) // niz sadrzaja procedura
    {
        for (int i = 0; i < procText.Length; i++)
        {
            string newProcText = "";
            string[] procLines = procText[i].Trim().Split('\n');

        }
    }

}


public class InstructionBlock : IExecutable
{
    public string codeText;
    string name;
    // List<string> par;
    Queue<IExecutable> instructions;
    Queue<IExecutable> insToExecute;
    Queue<IExecutable> insExecuted;
    IExecutable insCurrent;
    IConsoleToMap map;

    bool running = false;
    bool waitInsToFinish = false;

    public Queue<IExecutable> Instructions { get => instructions; }
    public int InsToExecuteCount { get => insToExecute.Count; }
    public int InsExecutedCount { get => insExecuted.Count; }
    public int InsTotalCount { get => insToExecute.Count + insExecuted.Count; }
    public IExecutable InsCurrent { get => insCurrent; }

    public InstructionBlock(string text, Queue<IExecutable> instructions)
    {
        try
        {
            // string[] temp = text.Trim().Split(':');
            // this.name = temp[0].Trim();
            // string procParamsDef = temp[1].Trim();

            // List<string> params;
            // // Odbacivanje praznih elemenata nastalih zbog vise uzastopnih blanko karaktera
            // string[] parStr = text.Split(' ');
            // foreach (var p in parStr)
            // {
            //     string tmp = p.Trim();
            //     if (tmp != "")
            //         paramsStr.Enqueue(tmp);
            // }

            // Original
            this.name = text;
            this.instructions = instructions;

            Reset();
        }
        catch (System.Exception)
        {    
            throw;
        }
    }

    public string Name() => name;

    void Reset()
    {
        ResetExecution();

        insToExecute = null;
        insExecuted = null;
        insCurrent = null;
    }

    public void ResetExecution()
    {
        running = false;
        waitInsToFinish = false;
        // Debug.Log("BLOCK @ " + name + ": RESET EXECUTION");
    }

    public void StopExecution()
    {
        // Debug.Log("BLOCK @ " + name + ": STOP EXECUTION");

        ConsoleController.UnsubscribeBlock(this);
        ResetExecution();
    }

    public void Execute(IConsoleToMap iMap)
    {
        map = iMap;
        StartExecution();
    }

    public void StartExecution()
    {
        Reset();

        // Debug.Log("BLOCK @ " + name + ": START EXECUTION");

        this.insToExecute = new Queue<IExecutable>();
        this.insExecuted = new Queue<IExecutable>();

        foreach (IExecutable ins in instructions)
            insToExecute.Enqueue(ins);

        ConsoleController.SubscribeBlock(this);
        running = true;
        waitInsToFinish = false;

        ExecuteInstruction();
    }

    void ExecuteInstruction()
    {
        // Poziv u nedozvoljenom trenutku
        if(!running || waitInsToFinish) return;

        waitInsToFinish = true;

        // Nema vise instrukcija za izvrsiti
        if(insToExecute.Count == 0)
        {
            // Debug.LogAssertion("Ins Count == 0");
            StopExecution();
            return;
        }

        insCurrent = insToExecute.Dequeue();
        // Debug.LogWarning("111");
        insCurrent.Execute(map);
        // Debug.LogWarning("222");
        // Debug.Log("BLOCK @ " + name + ": INSTRUCTION EXECUTED !!!");
        // Debug.Log("State: r = " + running + " w = " + waitInsToFinish);
    }

    public void NextInstruction()
    {
        if (!running || !ConsoleController.IsBlockOnStackTop(this))
            return;

        Debug.Log("BLOCK @ " + name + ": NEXT INSTRUCTION >>");
        insExecuted.Enqueue(insCurrent);

        waitInsToFinish = false;
        ExecuteInstruction();
    }
}

/* BACKUP - Pre renoviranja sistema koji je baziran na ConsoleController-u i prebacivanja na InstructionBlock sistem


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class ConsoleController : MonoBehaviour
{
    // Static
    public static ConsoleController Instance;
    public static void InstructionFinished()
    {
        // Instance.currentInstruction++;
        Instance.waitInsToFinish = false;
        Instance.ExecuteNextInstruction();
    }
    public static void InstructionInterrupted()
    {
        Instance.waitInsToFinish = false;
        Instance.running = false;
    }

    // UI
    public TMP_InputField inpMainCode;
    public Button btnRun;

    // Game References
    public MapController map;

    // Instructions
    InstructionNameMapping instructionNameMapping;
    
    // Local State
    string codeText;
    Queue<Instruction> instructions;
    // int currentInstruction = 0;
    bool running = false;
    bool waitInsToFinish = false;

    // Helpers
    //float counter = 0;

    // ===== Game =====

    void Awake()
    {
        // Set Singleton
        if(ConsoleController.Instance != null)
            GameObject.Destroy(ConsoleController.Instance);
        ConsoleController.Instance = this;
    }

    void Start()
    {
        // > Mappings
        InstructionNameMapping im = new InstructionNameMapping();
        
        im.Add(new string[] {"move", "мрдај"}, (s)=> new MoveInstruction(s));
        im.Add(new string[] {"pick", "покупи"}, (s)=> new PickInstruction(s));

        instructionNameMapping = im;
    }

    void Update()
    {

    }

    // ===== Methods ======

    public void RunCode()
    {
        codeText = inpMainCode.text;
        string[] codeLines = codeText.Split('\n');
        instructions = new Queue<Instruction>();

        foreach (string line in codeLines)
        {
            try
            {
                string[] ins = line.Split(':');
                ins[1] = ins[1].Trim();

                instructions.Enqueue(instructionNameMapping[ins[0]](ins[1]));
            }
            catch (InstructionException)
            {
                // Debug.Log("InstructionException");
                StopCodeExecution();
                return;
            }
        }

        waitInsToFinish = false;
        running = true;

        ExecuteNextInstruction();
    }

    public void StopCodeExecution()
    {
        running = false;
        waitInsToFinish = false;
    }


    void ExecuteNextInstruction()
    {
        // Poziv u nedozvoljenom trenutku
        if(!running || waitInsToFinish) return;

        waitInsToFinish = true;

        // Nema vise instrukcija za izvrsiti
        if(instructions.Count == 0)
        {
            StopCodeExecution();
            return;
        }

        instructions.Dequeue().Execute(map);
    }
}


class InstructionBlock
{
    Queue<Instruction> instructions;
    MapController map;

    // int currentInstruction = 0;
    bool running = false;
    bool waitInsToFinish = false;

    public InstructionBlock(MapController map, Queue<Instruction> instructions)
    {
        this.map = map;
        this.instructions = instructions;
    }
}


*/