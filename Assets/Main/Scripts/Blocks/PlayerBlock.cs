using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlock : MemoryBlock, IContainsItem
{
    // Item item;

    public override void InsertItem(Item item)
    {
        if (this.item != null) {
            GameObject.Destroy(TakeItem().gameObject);
            //throw new System.Exception("Player vec nosi Item!");
        }

        this.item = item;
        Debug.Log("Insert into Memory Block, item = " + this.item);

        item.transform.parent = this.transform;
        // item.transform.localPosition = new Vector3(0, this.transform.position.y + 1.2f, 0);
        // FinishAction();

        // Nova varijanta
        item.MovementFinished += delegate () {
            item.EmptyMovementFinishedEvent();
            FinishAction();
        };
        item.SetLocalPosition(new Vector3(0, this.transform.position.y + 1.2f, 0));
    }

    // public Item TakeItem()
    // {
    //     if (this.item == null)
    //         throw new System.Exception("Memory Block je prazan!");

    //     Item item = this.item;
    //     this.item = null;

    //     // FinishAction(); // Prebaceno u InsertItem u FunctionBlock i MemoryBlock

    //     return item;
    // }

    // public Item CloneItem()
    // {
    //     if (this.item == null)
    //         throw new System.Exception("Player ne nosi nista!");
    //     Item item = Instantiate(this.item.gameObject).GetComponent<Item>();

    //     Debug.Log("Copy from Memory Block, item = " + item);
    //     return item;
    // }

    // public bool HasItem()
    // {
    //     return item != null;
    // }

    // --------


}
