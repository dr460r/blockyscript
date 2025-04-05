using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8; // [unit/s]
    public float moveSpeedMultiplier = 1;
    public Vector3 targetLocalPosition;
    public bool updateLocalPosition;
    public event VoidFunc MovementFinished;

    [Header("Other")]
    public GameObject prefab;
    public CubeOverlayController cubeOverlay;
    public virtual Item New() => GameObject.Instantiate(prefab).GetComponent<Item>();

    public virtual Item Add(Item i) => null;
    public virtual Item Sub(Item i) => null;
    public virtual Item Mul(Item i) => null;
    public virtual Item Dev(Item i) => null;

    public virtual bool Compare(Item i) {
        Debug.LogWarning("ITEM COMPARE");
        return false;
    }

    public virtual bool Compare(string s) {
        Debug.LogWarning("ITEM COMPARE");
        return false;
    }

    void Awake()
    {
        cubeOverlay = GetComponentInChildren<CubeOverlayController>();
    }

    void Update()
    {
        if (updateLocalPosition) {
            //Debug.Log("Item Start Update");
            if (Vector3.Distance(targetLocalPosition, transform.localPosition) < 0.01f) {
                Debug.Log("Item Movement Finished, Event: " + (MovementFinished == null));
                updateLocalPosition = false;
                transform.localPosition = targetLocalPosition;
                MovementFinished?.Invoke();
            }
            else {
                Vector3 delta = (targetLocalPosition - transform.localPosition) * Time.deltaTime * moveSpeed * moveSpeedMultiplier;
                transform.localPosition += delta;
            }
            // Debug.Log("Item Distance From Target: " + Vector3.Distance(targetLocalPosition, transform.localPosition));
        }
    }

    public void EmptyMovementFinishedEvent()
    {
        Debug.Log("To Remove Subscribers");
        if (MovementFinished != null)
            foreach (var item in MovementFinished.GetInvocationList())
                MovementFinished -= (VoidFunc)item;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        Debug.Log("Item Set Local Position");
        targetLocalPosition = pos;
        updateLocalPosition = true;
    }
}
