using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatItem : Item
{
    public static int precision = 1;

    [SerializeField]
    public UnityEvent onOperationDone = new UnityEvent();

    [SerializeField]
    float value = 0;

    public override Item Add(Item i)
    {
        onOperationDone.Invoke(); // >> Event <<

        if (!(i is FloatItem)) return null;

        float x = (i as FloatItem).value;
        GameObject.Destroy(i.gameObject);
        return Set(this.value + x);
    }

    public override Item Sub(Item i)
    {
        onOperationDone.Invoke(); // >> Event <<

        if (!(i is FloatItem)) return null;

        float x = (i as FloatItem).value;
        GameObject.Destroy(i.gameObject);
        return Set(this.value - x);
    }

    public override Item Dev(Item i)
    {
        onOperationDone.Invoke(); // >> Event <<

        if (!(i is FloatItem)) return null;

        float x = (i as FloatItem).value;

        if (x == 0)
            throw new System.Exception("Deljenje nulom!!!!!");

        GameObject.Destroy(i.gameObject);
        return Set(this.value / x);
    }

    public override Item Mul(Item i)
    {
        onOperationDone.Invoke(); // >> Event <<

        if (!(i is FloatItem)) return null;

        float x = (i as FloatItem).value;
        GameObject.Destroy(i.gameObject);
        return Set(this.value * x);
    }

    public FloatItem Set(float val) 
    {
        value = Round(val);
        cubeOverlay.SetValue(value + "");
        return this;
    }

    public override bool Compare(Item i)
    {
        Debug.LogWarning("FLOAT ITEM COMPARE");

        if (!(i is FloatItem))
            return false;
        
        return (i as FloatItem).value == value;
    }

    public override bool Compare(string s)
    {
        Debug.LogWarning("FLOAT ITEM COMPARE (string): " + s);

        float x;
        if (!float.TryParse(s, out x))
            return false;
        
        return Round(x) == value;
    }

    public static float Round(float value) 
    {
        float k = Mathf.Pow(10, precision);
        return ((int)(value * k)) / k;
    }

    public override string ToString() => "" + value;
}
