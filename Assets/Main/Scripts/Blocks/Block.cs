using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void VoidFunc();

public class Block : MonoBehaviour
{
    // Public vars
    public MapController map;
    //public Vector2Int position { get; private set; }

    // General cofig
    public bool movable = false;
    public bool smoothMovement = false;
    public bool blockable = true;

    public float moveSpeed = 9; // [unit/s]
    public float moveSpeedMultiplier = 1;

    // State
    Vector3 targetLocalPosition;
    bool updateLocalPosition = false;
    // for teleport
    public GameObject localSinkFloorTile;
    Vector3 sinkOffset = new Vector3(0, -3f, 0);
    Vector3 lastLocalPosition;
    public bool endPhase;

    // Events
    public event VoidFunc ActionFinished;
    //public event VoidFunc ActionInterrupted;

    // For Smooth Movement Once
    public bool smoothMovementChanged = false;
    public bool smoothMovementBackup;


    /* ---- Engine ---- */

    void Start()
    {

    }

    void Update()
    {
        UpdateMovement();
    }

    /* ---- Initiate movement ---- */

    public void SetRealTargetPosition(Vector3 pos)
    {
        // Set real localPosition
        targetLocalPosition = pos;
        updateLocalPosition = true;

        // Debug.Log("SetRealTargetPisition(" + pos + ")");
    }

    /* ---- Realize movement ---- */

    public void UpdateMovement()
    {
        // Translate towards target world position / Teleport
        if(updateLocalPosition)
        {
            // Translate
            if(smoothMovement)
            {
                transform.localPosition = transform.localPosition + (targetLocalPosition - transform.localPosition) * moveSpeed * Time.deltaTime * moveSpeedMultiplier;
                if(Vector3.Distance(targetLocalPosition, transform.localPosition) < 0.01f) {
                    FinishMovement();
                }
            }
            // Teleport
            else {
                if (!endPhase) {
                    localSinkFloorTile?.SetActive(true);
                    Vector3 delta = (lastLocalPosition + sinkOffset - transform.localPosition) * moveSpeed * Time.deltaTime * moveSpeedMultiplier;
                    transform.localPosition += delta;
                    localSinkFloorTile.transform.localPosition -= delta;
                    if (Vector3.Distance(lastLocalPosition + sinkOffset, transform.localPosition) < 0.01f) {
                        endPhase = true;
                        transform.localPosition = targetLocalPosition + sinkOffset;
                    }
                }
                else {
                    Vector3 delta = (targetLocalPosition - transform.localPosition) * moveSpeed * Time.deltaTime * moveSpeedMultiplier;
                    transform.localPosition += delta;
                    localSinkFloorTile.transform.localPosition -= delta;
                    if(Vector3.Distance(targetLocalPosition, transform.localPosition) < 0.01f) {
                        FinishMovement();
                    }
                }
            }
        }
    }

    public void FinishMovement()
    {
        localSinkFloorTile?.SetActive(false);

        transform.localPosition = targetLocalPosition;
        updateLocalPosition = false;

        if (smoothMovementChanged)
        {
            endPhase = false;

            smoothMovementChanged = false;
            smoothMovement = smoothMovementBackup;
        }

        // Fire event for instruction end
        // Debug.LogWarning("Finish Movement");
        FinishAction();
    }

    public void FinishAction()
    {
        Debug.Log("Block Action Finished!");
        ActionFinished?.Invoke();
    }

    public void ChangeSmoothMovementOnce(bool sm) 
    {
        lastLocalPosition = transform.localPosition;
        endPhase = false;

        smoothMovementChanged = true;
        smoothMovementBackup = smoothMovement;
        smoothMovement = sm;
    }

}
