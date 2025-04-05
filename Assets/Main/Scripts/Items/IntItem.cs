using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntItem : Item
{
    [SerializeField]
    public UnityEvent onOperationDone = new UnityEvent();

    [SerializeField]
    int value = 0;

    public override Item Add(Item i)
    {
        if(i is IntItem) 
        {
            onOperationDone.Invoke(); // >> Event <<

            int x = (i as IntItem).value;
            GameObject.Destroy(i.gameObject);
            return Set(this.value + x);
        }

        return null;
    }

    public IntItem Set(int val) 
    {
        value = val;
        cubeOverlay.SetValue(val + "");
        return this;
    }

    public override string ToString() => "" + value;
}
