using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    Camera cam;

    // Backup - Initial Values
    public float mapHorizontalOffest;
    public float initFov;
    public Vector3 initPos;
    public Quaternion initRot;
    

    void Awake()
    {
        cam = GetComponent<Camera>();

        initFov = cam.fieldOfView;
        initPos = transform.position;
        initRot = transform.rotation;

        Debug.Log(initFov);
        Debug.Log(initPos);
        Debug.Log(initRot);
    }

    public void SetFOV(float fov)
    {
        cam.fieldOfView = fov;
    }

    public void SetupForMapPreview(Vector2 mapSize, Transform mapTransform)
    {
        Reset();
        
        this.gameObject.transform.position = new Vector3(0, 4f, mapSize.y);
        this.gameObject.transform.LookAt(mapTransform);

        float diagLen = Mathf.Sqrt(Mathf.Pow(mapSize.x, 2) + Mathf.Pow(mapSize.y, 2));
        this.gameObject.transform.Translate(new Vector3(diagLen*mapHorizontalOffest, 0, 0));
    }

    public void Reset()
    {
        cam.fieldOfView = initFov;
        this.gameObject.transform.position = initPos;
        this.gameObject.transform.rotation = initRot;
    }
}
