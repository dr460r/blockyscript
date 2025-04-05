using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionBlock : Block, IContainsItem
{   
    public FunctionBlockOperation operation = FunctionBlockOperation.ADD;
    public Stack<Item> stack = new Stack<Item>();

    void Execute()
    {
        Debug.Log("Function Block Execute");
        // 1 operand operations
        if(stack.Count == 1)
        {
            Debug.Log("One Operand Operation Goes Here");
        }
        // 2 operands operations
        else if(stack.Count == 2)
        {
            Debug.Log("||||| " + operation);
            if(operation == FunctionBlockOperation.ADD)
                Add();
            else if(operation == FunctionBlockOperation.SUB)
                Subtract();
            else if(operation == FunctionBlockOperation.MUL)
                Multiply();
            else if(operation == FunctionBlockOperation.DEV)
                Devide();
        }

        //Debug.Log("Stack Count: " + stack.Count);
        //Debug.Log("Stack Top: " + stack.Peek());
    }


    // ---- Interface ----

    public void InsertItem(Item item)
    {
        // PushOperand(item); // varijanta 1
        // item = stack.Peek(); // varijanta 1
        item.transform.parent = transform;
        
        // Stara varijanta
        // item.transform.localPosition = new Vector3(0, 1f, 0);
        // FinishAction();

        // Nova varijanta
        item.MovementFinished += delegate () { 
            PushOperand(item); 
            item.EmptyMovementFinishedEvent(); 
            FinishAction(); 
        }; // varijanta 3
        
        // item.MovementFinished += () => PushOperand(item); // varijanta 2
        // item.MovementFinished += FinishAction; // varijanta 2
        // item.MovementFinished += item.EmptyMovementFinishedEvent; // varijanta 2
        item.SetLocalPosition(new Vector3(0, 1f, 0));
    }

    public Item TakeItem()
    {
        Debug.Log("Poped from stack!");
        return stack.Pop();
    }

    public Item CloneItem()
    {
        // TODO Eentualno dodati proveru da stack slucajno nije prazan (mada ovako svakako baca exception)
        Debug.Log("Clone Item From Function Block");
        Item item = Instantiate(stack.Peek().gameObject).GetComponent<Item>();
        item.transform.position = stack.Peek().transform.position;
        return item;
    }

    public bool HasItem()
    {
        return stack.Count > 0;
    }

    public Item Peek()
    {
        return stack.Peek();
    }
    

    // ---- Methods ----

    public void PushOperand(Item item)
    {
        Debug.Log("Push Operand");
        stack.Push(item);
        Execute();
    }

    // ---- Function Block Types ----

    // 1) Addition Block
    void Add()
    {
        Item b = stack.Pop();
        Item a = stack.Pop();
        Item c = a.Add(b);

        //Debug.Log("Add: " + a + ", " + b);
        stack.Push(c);
    }

    // 2) Subtraction Block
    void Subtract()
    {
        Item b = stack.Pop();
        Item a = stack.Pop();
        Item c = a.Sub(b);

        //Debug.Log("Sub: " + a + ", " + b);
        stack.Push(c);
    }

    // 3) Multiplication Block
    void Multiply()
    {
        Item b = stack.Pop();
        Item a = stack.Pop();
        Item c = a.Mul(b);

        //Debug.Log("Mul: " + a + ", " + b);
        stack.Push(c);
    }
    
    // 4) Devision Block
    void Devide()
    {
        Item b = stack.Pop();
        Item a = stack.Pop();
        Item c = a.Dev(b);

        //Debug.Log("Dev: " + a + ", " + b);
        stack.Push(c);
    }

    

}

public enum FunctionBlockOperation {
    ADD,
    SUB, 
    MUL, 
    DEV
}
