using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer : IMovable
{
    void PickItem(Block source);
}