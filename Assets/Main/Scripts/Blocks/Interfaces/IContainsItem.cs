using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainsItem
{
    void InsertItem(Item item);
    Item TakeItem();
    Item CloneItem();
    Item Peek();
    bool HasItem();
}
