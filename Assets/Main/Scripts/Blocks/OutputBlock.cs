using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputBlock : MemoryBlock, IContainsItem
{   
    public event VoidFunc EventGoalCompleted;

    public string goal;
    public bool GoalCompleted { get => item != null && item.Compare(goal); }

    public override void InsertItem(Item item)
    {
        Debug.LogWarning("OUT BLOCK || OLD ITEM: " + this.item);
        Debug.LogWarning("OUT BLOCK >> INSER ITEM: " + item);
        base.InsertItem(item);
         Debug.LogWarning("OUT BLOCK || NEW ITEM: " + this.item);

        Debug.LogWarning("OUT BLOCK ## GOALCOMPLETED: " + GoalCompleted);
        if (GoalCompleted) {
            Debug.LogWarning("GOOOOOOAL");
            EventGoalCompleted?.Invoke();
        }
    }

    public void SetGoal(string s) {
        goal = s;
    }
}
