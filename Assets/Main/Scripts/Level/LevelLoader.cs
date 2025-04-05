using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{   
    //public static int itemFloatPrecision = 1;

    [Header("Dev")]
    public bool toCleanTrash;

    [Header("Configuration")]
    public bool loadFromFile = true;
    public string mapName = "Original";

    [Header("Global Prefabs")]
    public GameObject goMap;
    public GameObject goConsole;
    public GameObject goSelectMapBtn;

    public MapController map;
    public ConsoleController console;

    [Header("Map Elements Prefabs")]
    public GameObject tileEmpty;
    public GameObject tileTeleport;
    public GameObject intItem;
    public GameObject floatItem;
    
    // > Base blocks
    // public GameObject blockPlayer;
    // public GameObject blockWall;

    // > Special blocks
    // public GameObject blockFnAdd;
    // public GameObject blockFnSub;
    // public GameObject blockFnMul;
    // public GameObject blockFnDev;

    // UI
    public MainMenuController canvas;
    public GameObject pnlUI;
    public TextMeshProUGUI choosedMapName;
    public GameObject pnlMaps;
    public List<Button> btnSelectMap;

    // Map file mappings
    public MapFileMappings blockMappingsConfig;
    public Dictionary<char, GameObject> blockMappings;

    // Singleton
    public static LevelLoader Instance { get; set; }

    // Cleaner
    public GameObject doNotDestoryContainer;
    public int cleanOnObjectCount;
    public float timeTick; // Vreme koje treba da prodje da bi se izvrsile akcije u Update-u koje je potrebno povremeno izvrsavati
    public float timeCounter = 0;

    void Awake()
    {
        // Singleton
        if (LevelLoader.Instance != null)
             GameObject.Destroy(LevelLoader.Instance);
        LevelLoader.Instance = this;
    }

    void Start()
    {
        ResetAndLoadForHome();
        // InitLoading();

        // DEBUG

        // int n = 3;
        // GameObject[,] fl = new GameObject[n,n];
        // GameObject[,] bl = new GameObject[n,n];

        // for (int i = 0; i < n; i++)
        //     for (int j = 0; j < n; j++) {
        //         fl[i,j] = tileEmpty;
        //         bl[i,j] = null;
        //     }
    
        // fl[0,0] = null;
        // bl[2,1] = blockFnAdd;
        // bl[2,0] = blockPlayer;

        // map.Generate(new Vector2Int(n,n), fl, bl);
    }

    void Update()
    {
        // Clean - Manual method
        if (toCleanTrash) 
        {
            toCleanTrash = false;
            CleanTrash();
        }

        // Clean - Automatic method - OVO NE SME, BRISE INSTANCE ITEM-a KOJE SE NELAZE U loadedData U MAPI
    }

    public void ResetAndLoadForHome()
    {
        Reset();
        mapName = btnSelectMap[Random.Range(0, btnSelectMap.Count-1)].gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        LoadMap(true);
    }

    public void QuitLevel()
    {
        map.QuitThisMap();
    }

    public void Reset()
    {
        CleanTrash();

        if (map != null) GameObject.Destroy(map.gameObject);
        if (console != null) GameObject.Destroy(console.gameObject);

        blockMappings = new Dictionary<char, GameObject>();
        btnSelectMap = new List<Button>();

        LoadUI();
        pnlUI.SetActive(true);
    }

    public void SetMapName(string name) 
    {
        // Reset(); // Za Rest Preview-a

        mapName = name;
        choosedMapName.text = name; // << Update UI Element

        LoadMap(true); // Za Load Preview-a
    }

    public void LoadUI()
    {
        // Ciscenje liste
        foreach (var item in pnlMaps.GetComponentsInChildren<Button>()) {
            Debug.LogWarning("Obrisano dugmence");
            Destroy(item.gameObject);
        }

        // Ucitavanje imena dostupnih mapa
        string[] mapNames = AssetTextIO.Instance.GetMapNames();

        btnSelectMap = new List<Button>();
        foreach (var mn in mapNames)
        {
            Debug.LogWarning("Mapa: " + mn);
            Button btn = Instantiate(goSelectMapBtn, pnlMaps.transform).GetComponent<Button>();
            btn.onClick.AddListener(()=>canvas.ChooseMap(mn));
            btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = mn;

            btnSelectMap.Add(btn);
        }

        // Postavljanje visine kontejnera koji sadrzi kreirane dugmice (zbog scroll-a)
        float btnHeight = goSelectMapBtn.GetComponent<RectTransform>().sizeDelta.y;
        RectTransform contentRect = pnlMaps.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, btnSelectMap.Count*(btnHeight+2));
    }

    public void LoadMap(bool previewMode = false)
    {
        Reset();

        // UI
        if (!previewMode) {
            pnlUI.SetActive(false);
        }

        // Setup
        map = GameObject.Instantiate(goMap).GetComponent<MapController>();
        map.transform.SetParent(doNotDestoryContainer.transform);
        
        if (previewMode) {
            map.previewMode = true;
        }
        
        if (!previewMode) {
            console = GameObject.Instantiate(goConsole).GetComponent<ConsoleController>();
            console.transform.SetParent(doNotDestoryContainer.transform);
            console.map = map;
        }
        
        // Loadings
        LoadBlockMappings();
        LoadMap(mapName);
    }

    private void LoadBlockMappings()
    {
        foreach (var item in blockMappingsConfig.mappings)
        {
            blockMappings.Add(item.key, item.value);
        }
    }

    private List<string> LoadMapTextLinesFromFile(string mapName)
    {
        List<string> list = new List<string>();
        string[] lines = AssetTextIO.Instance.ReadMap(mapName).Split('\n', '\r');
        
        foreach (var line in lines)
        {
            // Debug.Log(line);
            if (line.Length > 0)
                list.Add(line.TrimEnd(' ', '\t'));
        }

        return list;
    }

    private List<string> LoadMapTextLines(string mapName)
    {
        // TODO
        List<string> list = new List<string>();
        // string[] lines = AssetTextIO.Instance.ReadMap(mapName).Split('\n', '\r');
        
        // foreach (var line in lines)
        // {
        //     if (line.Length > 0)
        //         list.Add(line.TrimEnd(' '));
        // }

        return list;
    }

    private void LoadMap(string mapName)
    {
        MapData md = new MapData();
        List<string> lines = (loadFromFile) ? LoadMapTextLinesFromFile(mapName) : LoadMapTextLines(mapName);
        Vector2Int mapSize = new Vector2Int();
        md.floor = null;
        md.blocks = null;
        
        int i = 0;
        while (i < lines.Count)
        {
            if (lines[i][0] == '@') 
            {
                string section = lines[i].TrimStart('@').Split(' ')[0];
                // Debug.Log("Section: " + section);

                if (section == "Map")
                {
                    string[] tmp = lines[i].Split(' ')[1].Split('x');
                    mapSize.Set(int.Parse(tmp[0]), int.Parse(tmp[1]));
                }
                else if (section == "Blocks")
                {
                    i++;
                    md.floor = new GameObject[mapSize.x, mapSize.y];
                    md.blocks = new GameObject[mapSize.x, mapSize.y];
                    md.itemsConf = new GameObject[mapSize.x, mapSize.y];

                    string DEBUG_STRING = "\n";
                    for (int y = 0; y < mapSize.y; y++) 
                    {
                        // string[] row = lines[i + y].Split(' ');
                        string[] row = lines[i + y].ToLower().Split(' ');
                        for (int x = 0; x < mapSize.x; x++) 
                        {
                            GameObject obj = blockMappings[row[x][0]];
                            md.blocks[x,y] = (obj != tileEmpty && obj != null)? obj : null;
                            md.floor[x,y] = (obj != null)? ((obj.GetComponent<PlayerBlock>() != null)? tileTeleport : tileEmpty) : null;

                            DEBUG_STRING += row[x] + " ";
                        }
                        DEBUG_STRING += "\n";
                    }
                    // Debug.Log(DEBUG_STRING);

                    i += mapSize.y - 1;
                }
                else if (section == "Items")
                {
                    i++;
                    GameObject trash = new GameObject();
                    trash.transform.position = new Vector3(1000,1000,1000);

                    Dictionary<char, float> itemsMaping = new Dictionary<char, float>();

                    // Load Variable Mappings
                    while (lines[i].Contains(":"))
                    {
                        if (lines[i].Split(':').Length == 2) {
                            char key = lines[i].Split(':')[0].Trim()[0];
                            float val = float.Parse(lines[i].Split(':')[1].Trim());

                            itemsMaping.Add(key, val);
                        }
                        else {
                            // > Base Parse
                            string[] s = lines[i].Split(':');
                            char key = char.Parse(s[0].Trim());
                            string valRange = s[1].Trim();
                            string valSet = s[2].Trim();

                            // > Decide Value
                            // Ako je prazan [] ili {} niz ce imati jedan element, i to ""
                            string[] valRangeNums = valRange.Replace("[","").Replace("]","").Split(',');
                            string[] valSetNums = valSet.Replace("{","").Replace("}","").Split(',');
                            
                            List<float> range = new List<float>();
                            if (valRangeNums[0] != "") {
                                range.Add(float.Parse(valRangeNums[0]));
                                range.Add(float.Parse(valRangeNums[1]));
                            }

                            List<float> set = new List<float>();
                            if (valSetNums[0] != "") {
                                foreach (var item in valSetNums) {
                                    string modItem = item.Replace(".", ",");
                                    Debug.LogWarning("Set Parsing: " + item + " (" + modItem + ")" + " -> " + float.Parse(modItem));
                                    set.Add(float.Parse(modItem));
                                }
                            }

                            float rangeProb = -1;
                            // Both Range and Set are defined
                            if (range.Count == 2 && set.Count > 0) {
                                int rn = (int) (range[1] - range[0]);
                                rangeProb = rn / (float)(set.Count + rn);
                                Debug.LogWarning("Calculated Range Probability: " + rangeProb);
                            }
                            // Only Set is defined
                            else if(range.Count == 0 && set.Count > 0) {
                                rangeProb = 0;
                            }
                            // Only Range is defined
                            else if (range.Count == 2 && set.Count == 0) {
                                rangeProb = 1;
                            }
                            // Both Range and Set are undefinde
                            else {
                                // Default Range
                                rangeProb = 1;
                                range.Add(0);
                                range.Add(10);
                            }

                            float val;
                            // Pick Variable Value from Range
                            if (Random.Range(0f,1f) < rangeProb) 
                            {
                                val = Random.Range((int)range[0], (int)(range[1] + 1));
                            }
                            // Pick Variable Value from Set
                            else 
                            {
                                int index = Random.Range(0, set.Count);
                                val = set[index];
                            }

                            itemsMaping.Add(key, val);
                        }

                        i++;
                    }

                    // Apply Variable Values
                    string DEBUG_STRING = "\n";
                    for (int y = 0; y < mapSize.y; y++) 
                    {
                        string[] row = lines[i + y].Split(' ');
                        for (int x = 0; x < mapSize.x; x++)
                        {
                            GameObject obj = null;
                            if (itemsMaping.ContainsKey(row[x][0])) {
                                obj = GameObject.Instantiate(floatItem, trash.transform);
                                obj.GetComponent<FloatItem>().Set(itemsMaping[row[x][0]]); // << INT ITEM / FLOAT ITEM
                            }

                            md.itemsConf[x,y] = obj;

                            DEBUG_STRING += row[x] + " ";
                        }
                        DEBUG_STRING += "\n";
                    }
                    // Debug.Log(DEBUG_STRING);

                    i += mapSize.y;

                    // > Goal
                    string e = lines[i];
                    // Zamena znakova za vrednost varijabli, unutar izraza za Goal
                    foreach (var item in itemsMaping)
                    {
                        e = e.Replace(item.Key + "", item.Value + "");
                    }
                    md.goalExpression = e;
                    md.goal = CalculateGoal(e);
                    // Debug.LogWarning("FINAL GOAL: " + md.goal);
                    md.goal = FloatItem.Round(float.Parse(md.goal)) + ""; // <<< OVO NIKAKO NE VALJA, POTREBNO JE DRUGACIJE PRISTUPITI ZAOKRUZIVANJU (ILI GA IZBACITI ODAVDE, NESTO MORA). Ali, ipak ce morati ovako... :(
                    // Debug.LogWarning("FINAL GOAL: " + md.goal);
                }
                // else Debug.LogError("Map file syntax problem!");
            }

            i++;
        }

        if (md.floor != null && md.blocks != null) {
            md.size = new Vector2Int(mapSize.x, mapSize.y);
            md.mapName = mapName;
            map.Generate(AdaptMapDataAxisDir(md));
        }
        // else
            // Debug.LogError("Map not loaded!");
    }

    private MapData AdaptMapDataAxisDir(MapData md)
    {
        // Blocks
        GameObject[,] tmpBlocks = new GameObject[md.size.x, md.size.y];
        GameObject[,] tmpFloor = new GameObject[md.size.x, md.size.y];
        GameObject[,] tmpItemsConf = new GameObject[md.size.x, md.size.y];
        
        for (int y = 0; y < md.size.y; y++) {
            for (int x = 0; x < md.size.x; x++) 
            {
                int modX = md.size.x - x - 1;
                int modY = y;

                tmpBlocks[modX, modY] = md.blocks[x,y];
                tmpFloor[modX, modY] = md.floor[x,y];
                tmpItemsConf[modX, modY] = md.itemsConf[x,y];
            }
        }
        
        md.blocks = tmpBlocks;
        md.floor = tmpFloor;
        md.itemsConf = tmpItemsConf;

        return md;
    }

    private string CalculateGoal(string startStr) 
    {
        // Debug.LogWarning(startStr);
        string resultStr = startStr;
        // Regex rxNum = new Regex(@"^\-?[0-9]*\,?[0-9]+$");
        // Regex rxGr = new Regex(@"\(\-?[0-9]*\,?[0-9]+[\+\-\*\/][0-9]*\,?[0-9]+\)");
        // Regex rxOp = new Regex(@"[0-9]*\,?[0-9]+([\+\-\*\/])[0-9]*\,?[0-9]+");
        // Regex rxA = new Regex(@"\(\-?[0-9]*\,?[0-9]*");
        // Regex rxB = new Regex(@"[0-9]*\,?[0-9]+\)");
        string rxStrNum = @"\-?[0-9]*\,?[0-9]+";
        string rxStrExp = "(" + rxStrNum + @")([\+\-\*\/\:])(" + rxStrNum + ")";
        Regex rxOneNum = new Regex("^" + rxStrNum + "$");
        Regex rxExpGr = new Regex(@"\(" + rxStrExp + @"\)");

        while (!rxOneNum.Match(resultStr).Success)
        {
            GroupCollection groups = rxExpGr.Match(resultStr).Groups;
            Debug.LogWarning(groups[0].Value);
            float a = float.Parse(groups[1].Value);
            float b = float.Parse(groups[3].Value);
            char op = groups[2].Value[0];
            
            float x = 0;
            if (op == '+') x = a + b;
            else if (op == '-') x = a - b;
            else if (op == '*') x = a * b;
            else if (op == '/' && b != 0) x = a / b; // Kad nema deljenja nulom
            else if (op == ':' && b != 0) x = (int)(a / b); // Celobrojno deljenje
            else if ((op == '/' || op == ':') && b == 0) x = 0; // Resenje za Deljenje nulom (Lose ali radi)
            //else if (op == '/' && b == 0) x = a >= 0 ? float.MaxValue : -float.MaxValue; // Deljenje nulom (TREBALO BI OVAKO, ALI +E FORMAT PRAVI PROBLEM)
            // else if (op == '/') x = a / b; // => Generise problem Zero Dev
            
            resultStr = resultStr.Replace(groups[0].Value, "" + x).Replace("--", "+").Replace("+-", "-");
            Debug.LogWarning(resultStr);
        }

        return resultStr;
    }

    public void CleanTrash()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var item in rootObjects)
        {
            if (item != doNotDestoryContainer && item != this.gameObject) {
                GameObject.Destroy(item);
            }
            // Debug.Log(item);
        }
    }
    
}
