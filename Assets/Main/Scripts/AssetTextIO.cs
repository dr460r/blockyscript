using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AssetTextIO
{
    // Singleton setup

    private static AssetTextIO instance;

    private AssetTextIO() { }

    public static AssetTextIO Instance
    {
        get => (instance == null)? instance = new AssetTextIO() : instance;
    }

    // Config
    private string mapsDataPath = "Maps/";
    private string mapFileExtension = "mapa";

    // Readers

    private string Read(string path) 
    {
        return File.ReadAllText(Application.streamingAssetsPath  + "/" + path);
    }


    public string ReadMap(string mapName)
    {
        return Read(mapsDataPath + mapName + "." + mapFileExtension);
    }

    public string[] GetMapNames()
    {
        string[] paths = Directory.GetFiles(Application.streamingAssetsPath  + "/" + mapsDataPath, "*." + mapFileExtension + "", SearchOption.TopDirectoryOnly);

        // Regex rxFileName = new Regex(@"\/([A-Za-z0-9_\ \-]+)(\." + mapFileExtension + ")");
        Regex rxFileName = new Regex(@"\/([^\/]+)(\." + mapFileExtension + ")");
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = rxFileName.Match(paths[i]).Groups[1].Value;
        }

        return paths;
    }
}
