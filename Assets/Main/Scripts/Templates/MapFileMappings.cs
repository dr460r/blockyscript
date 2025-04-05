using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map File Mappings", fileName = "NewMappings")]
public class MapFileMappings : ScriptableObject
{
    public Mapping[] mappings;
}

[System.Serializable]
public class Mapping 
{
    public char key;
    public GameObject value;
}
