using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeOverlayController : MonoBehaviour
{
    public void SetValue(string value) 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = value;
        }
    }
}
