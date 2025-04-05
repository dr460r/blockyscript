using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryBlock : Block, IContainsItem
{   
    public Item item = null;

    // ---- Interface ----

    public virtual void InsertItem(Item item)
    {
        if (this.item != null) {
            GameObject.Destroy(TakeItem().gameObject);
            //throw new System.Exception("Memory Block je pun!"); // STARO
        }

        this.item = item;
        Debug.Log("Insert into Memory Block, item = " + this.item);

        item.transform.parent = transform;
        // item.transform.localPosition = new Vector3(0, 1f, 0);
        // FinishAction();
        // Nova varijanta
        item.MovementFinished += delegate () {
            item.EmptyMovementFinishedEvent();
            FinishAction();
        };
        item.SetLocalPosition(new Vector3(0, 1f, 0));
    }

    public Item TakeItem()
    {
        if (this.item == null)
            throw new System.Exception("Memory Block je prazan!");

        Item item = this.item;
        this.item = null;

        Debug.Log("Pick from Memory Block, item = " + item);
        return item;
    }

    public Item CloneItem()
    {
        if (this.item == null)
            throw new System.Exception("Memory Block je prazan!");

        Item item = Instantiate(this.item.gameObject).GetComponent<Item>();
        item.transform.position = this.item.transform.position;

        Debug.Log("Copy from Memory Block, item = " + item);
        return item;
    }

    public bool HasItem()
    {
       return this.item != null;
    }

    public Item Peek()
    {
        return item;
    }
    
}
