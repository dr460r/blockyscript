using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class MapController : MonoBehaviour, IConsoleToMap
{
    [Header("TEMP")]
    public GameObject IntTilePrefab;

    [Header("Prefabs")]
    public GameObject floorTilePrefab;
    public GameObject playerPrefab;

    [Header("Settings")]
    public bool previewMode;
    public float virtToRealPositionRatio;
    public Vector2Int mapSize;
    public float globalMoveSpeedMultiplier = 1;

    [Header("UI")]
    public Canvas uiCanvas;
    public Canvas canvasPlane;
    public TextMeshProUGUI txtGoal;
    public TextMeshProUGUI txtMapName;
    public TextMeshProUGUI txtGoalExpression;
    public TextMeshProUGUI txtSpeed;
    public TextMeshProUGUI txtN;
    public TextMeshProUGUI txtS;
    public TextMeshProUGUI txtE;
    public TextMeshProUGUI txtW;

    // [Header("Other")]
    // public Camera mapCamera;

    /* -------- State -------- */
    GameObject[,] floor;
    Block[,] blocks;
    PlayerBlock player;
    public OutputBlock output;

    public GameObject pnlGameFinished;
    MapData loadedData;
    GameObject floorContainer;
    GameObject blocksContainer;

    public Vector3 tileSize;
    float counter = 0f;
    public Vector2Int InitPlayerPos { get; private set; }
    public Vector2Int CurrentPlayerPos 
    {
        get 
        {
            for (int y = 0; y < mapSize.y; y++) 
                for (int x = 0; x < mapSize.x; x++)
                    if (blocks[x,y] is PlayerBlock)
                        return new Vector2Int(x,y);

            return new Vector2Int(-1,-1);
        }
    }

    /* ======== Methods ======== */
    
    /* -------- Game -------- */

    void Start()
    {
        // mapCamera = Camera.main;
        // GenerateBase(); // Generise pod
        // GenerateBlocks();
        // InstantiatePlayer(new Vector2Int(1,2));
        
    }

    void Update()
    {
        if (previewMode)
            this.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * 10f);

        // counter += Time.deltaTime;
        // if(counter < 2f) return;
        // counter = 0;

        // IntItem item = GameObject.Instantiate(IntTilePrefab).GetComponent<Item>() as IntItem;
        // item.onOperationDone.AddListener(()=>Debug.Log(">>>>>>>> SABRANOOOOOO <<<<<<<<<<"));
        // ((FunctionBlock)blocks[2,2]).PushOperand(item);
        // Debug.Log("+2s Update");
        // Debug.Log(player.position + new Vector2Int(1,0));
        // player.Move(player.position + new Vector2Int(1,0));

    }

    /* -------- Map Generation -------- */

    public void QuitThisMap() 
    {
        LevelLoader.Instance.ResetAndLoadForHome();
    }

    public void FinishGame() 
    {
        pnlGameFinished.SetActive(true);
    }

    public void Regenerate()
    {
        GameObject.Destroy(floorContainer);
        GameObject.Destroy(blocksContainer);
        Generate(loadedData);
        //Generate(loadedData.size, loadedData.floor, loadedData.blocks, loadedData.itemsConf);
    }

    public void Generate(MapData data)
    {
        if (loadedData != data) {
            Debug.Log("Menja!!!");
            loadedData = data;
        }
        else {
            foreach (var item in data.itemsConf)
            {
                Debug.Log(item);
            }
        }

        // Generation
        Generate(data.size, data.floor, data.blocks, data.itemsConf);

        // UI
        if (!previewMode) {
            txtGoal.text = loadedData.goal;
            txtMapName.text = loadedData.mapName;
            if (loadedData.goalExpression[0] == '(')
                txtGoalExpression.text = loadedData.goalExpression.Remove(0,1).Remove(loadedData.goalExpression.Length-2,1);
            else
                txtGoalExpression.text = loadedData.goalExpression;
        }
        else {
            uiCanvas.gameObject.SetActive(false);
            canvasPlane.gameObject.SetActive(false);
        }
        int gsmmRem = (int)(globalMoveSpeedMultiplier * 10) % 10;
        txtSpeed.text = ("x" + globalMoveSpeedMultiplier).Replace(",", ".") + (gsmmRem == 0 ? ".0" : "");

        // Camera
        MapCameraController mapCam = Camera.main.GetComponent<MapCameraController>();
        if (mapCam != null && !previewMode) {
            mapCam.Reset();
            if (mapSize.x >= mapSize.y) {
                mapCam.SetFOV(mapSize.x * 7 + 70 / mapSize.x);
            }
            else {
                mapCam.SetFOV(mapSize.x * 7 + 70 / mapSize.x); // << ISPRAVITI  FOV ZA VEOMA "VISOKE" MAPE
            }
        }
        else if (mapCam != null) {
            mapCam.SetupForMapPreview(mapSize, transform);
        }
    }

    public void Generate(Vector2Int size, GameObject[,] floorData, GameObject[,] blocksData, GameObject[,] itemsConf)
    {
        mapSize = size;

        // > Base

        blocks = new Block[mapSize.x, mapSize.y];
        floor = new GameObject[mapSize.x, mapSize.y];
        floorContainer = GameObject.Instantiate(new GameObject("GO"), this.transform);
        floorContainer.name = "Floor";
        
        for (int y = 0; y < mapSize.y; y++) 
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if(floorData[x,y] != null)
                {
                    GameObject newTile = GameObject.Instantiate(floorData[x,y], floorContainer.transform);
                    // Debug.Log(newTile);
                    newTile.name = "Floor Tile " + (y * mapSize.x + x);
                    // newTile.transform.localPosition = new Vector3(x, -tileSize.y/2f, y); // Stari nacin
                    newTile.transform.localPosition = GetRealTilePosition(x, y, 0);
                    floor[x,y] = newTile;
                }
                
                blocks[x,y] = null;
            }
        }


        // > Blocks

        blocksContainer = GameObject.Instantiate(new GameObject("GO"), this.transform);
        blocksContainer.name = "Blocks";
        blocksContainer.SetActive(false);

        for (int y = 0; y < mapSize.y; y++) 
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if(blocksData[x,y] != null)
                {
                    Block newBlock = GameObject.Instantiate(blocksData[x,y], blocksContainer.transform).GetComponent<Block>();
                    newBlock.moveSpeedMultiplier = globalMoveSpeedMultiplier;
                    
                    // Insert Item
                    if (newBlock is IContainsItem && itemsConf[x,y] != null) {
                        Item item = Instantiate(itemsConf[x,y]).GetComponent<Item>();
                        item.moveSpeedMultiplier = globalMoveSpeedMultiplier;
                        (newBlock as IContainsItem).InsertItem(item);
                    }

                    if(newBlock is PlayerBlock)
                    {
                        newBlock.name = "Player";
                        player = newBlock as PlayerBlock;
                        player.map = this;
                        InitPlayerPos = new Vector2Int(x,y);
                    }
                    else if(newBlock is FunctionBlock) 
                    {
                        newBlock.name = "Function";
                    }
                    else if(newBlock is OutputBlock) 
                    {
                        newBlock.name = "Output";
                        output = newBlock as OutputBlock;
                        output.SetGoal(loadedData.goal);
                        output.EventGoalCompleted += FinishGame;
                    }
                    else if(newBlock is MemoryBlock) 
                    {
                        newBlock.name = "Memory";
                    }
                    
                    newBlock.name += " Block " + (y * mapSize.x + x);
                    // newBlock.transform.localPosition = new Vector3(x, tileSize.y/2f, y); // Stari nacin
                    newBlock.transform.localPosition = GetRealTilePosition(x, y, 1);

                    // Event Callback
                    if (ConsoleController.Instance != null)
                        newBlock.ActionFinished += () => ConsoleController.InstructionFinished();


                    blocks[x, y] = newBlock;
                }
            }
        }


        blocksContainer.SetActive(true);

        // Dodatna podesavanja
        Vector3 p1 = GetRealTilePosition(0, 0);
        Vector3 p2 = GetRealTilePosition(mapSize.x-1, mapSize.y-1);
        float distX = (p1.x - p2.x) / 2f;
        float distZ = (p1.z - p2.z) / 2f;
        Vector3 fixedContPos = new Vector3(p1.x + distX, 0, p1.z + distZ);
        blocksContainer.transform.localPosition = fixedContPos;
        floorContainer.transform.localPosition = fixedContPos;

        txtN.rectTransform.localPosition = new Vector3(0, -distZ + tileSize.y * 2f, 0);
        txtS.rectTransform.localPosition = new Vector3(0, distZ - tileSize.y * 1.3f, 0);
        txtE.rectTransform.localPosition = new Vector3(-distX + tileSize.x * 1.5f, 0, 0);
        txtW.rectTransform.localPosition = new Vector3(distX - tileSize.x * 1.5f, 0, 0);
        


        // DEBUG
        // IntItem item = GameObject.Instantiate(IntTilePrefab).GetComponent<Item>() as IntItem;
        // ((FunctionBlock)blocks[2,1]).PushOperand(item);

    }

    Vector3 GetRealTilePosition(int x, int y, int layer = 0)
    {
        return new Vector3(x * tileSize.x, layer * tileSize.y - tileSize.y / 2f, y * tileSize.z);
    }


    /* -------- UI -------- */

    public void ShowGoalExpression()
    {
        txtGoalExpression.enabled = true;
    }

    /* -------- Controll --------*/
    public void IncreseGlobalMoveSpeedMultiplier(bool speedUp)
    {
        float gsmm = globalMoveSpeedMultiplier;

        if ((gsmm == 6f && speedUp) || (gsmm < 0.3f && !speedUp)) return;
        globalMoveSpeedMultiplier += speedUp ? (gsmm >= 1f ? 1f : 0.2f) : (gsmm > 1f ? -1f : -0.2f);
        
        int gsmmRem = (int)(globalMoveSpeedMultiplier * 10) % 10;
        txtSpeed.text = ("x" + globalMoveSpeedMultiplier).Replace(",", ".") + (gsmmRem == 0 ? ".0" : "");

        foreach (var block in blocks)
        {
            if (block == null)
                continue;

            block.moveSpeedMultiplier = globalMoveSpeedMultiplier;

            if (block is IContainsItem && (block as IContainsItem).HasItem()) {
                (block as IContainsItem).Peek().moveSpeedMultiplier = globalMoveSpeedMultiplier;
            }
        }
    }


    /* -------- Blocks -------- */

    public bool FindPositionOfBlock(Block block, out Vector2Int pos)
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if(block == blocks[x,y])
                {
                    // Debug.Log("Block found: " + blocks[x,y]);
                    pos = new Vector2Int(x,y);
                    return true;
                }
            }
        }

        pos = new Vector2Int(-1, -1);
        return false;
    }

    public bool MoveBlock(Block trgBlock, Vector2Int dir, bool jump)
    {
        // Pronalazak bloka za pomeranje
        Vector2Int oldPos;

        if(!FindPositionOfBlock(trgBlock, out oldPos))
            return false;

        Vector2Int newPos = oldPos + dir;

        // Pokusaj pomeranja na nedozvoljeno mesto
        if (!IsLocationWalkable(newPos, trgBlock))
        {
            // ** Moze i preko Event Callback-a
            // Debug.LogWarning("Move/Jump instruction failed!");
            Debug.Log("Ciljna pozicija nije prohodna!");
            ConsoleController.InstructionInterrupted();
            return false;
        }

        // Provera za blokove izmedju, tj za celu predjenu putanju
        // Horizontalno
        if (!jump && oldPos.x != newPos.x && oldPos.y == newPos.y) 
        {
            int hPom = oldPos.x < newPos.x ? 1 : -1;
            for (int i = oldPos.x + hPom; i != newPos.x; i += hPom) 
            {
                if (!IsLocationWalkable(new Vector2Int(i, oldPos.y), trgBlock)) 
                {
                    Debug.Log("Horizontalni pomeraj ne valja!");
                    ConsoleController.InstructionInterrupted();
                    return false;
                }
            }
        }
        // Vertikalno
        else if(!jump && oldPos.y != newPos.y && oldPos.x == newPos.x)
        {
            int vPom = oldPos.y < newPos.y ? 1 : -1;
            for (int i = oldPos.y + vPom; i != newPos.y; i += vPom)
            {
                if (!IsLocationWalkable(new Vector2Int(oldPos.x, i), trgBlock))
                {
                    Debug.Log("Vertikalni pomeraj ne valja!");
                    ConsoleController.InstructionInterrupted();
                    return false;
                }
            }
        }

        Debug.Log("Check If Can Move: ALL CHECKS PASSED!!!");

        // Pomeraj na virtuelnomj mapi (blocks)
        blocks[newPos.x, newPos.y] = blocks[oldPos.x, oldPos.y];
        if (oldPos != newPos) // ako se radi jump, izgubice se player (jer je oldPos == newPos)
            blocks[oldPos.x, oldPos.y] = null;
        if (jump) trgBlock.ChangeSmoothMovementOnce(false);
        trgBlock.SetRealTargetPosition(new Vector3(newPos.x, 0, newPos.y)*virtToRealPositionRatio + new Vector3(0, tileSize.y/2f, 0));
        // Debug.LogWarning("Moved!");
        return true;
    }

    public bool IsLocationWalkable(Vector2Int newPos, Block trgBlock)
    {
        bool cInBounds = CheckMapBounds(newPos);
        bool cHasFloor = floor[newPos.x, newPos.y] != null;
        bool cNotBlockable = (blocks[newPos.x, newPos.y] != null && !blocks[newPos.x, newPos.y].blockable) || blocks[newPos.x, newPos.y] == null;
        bool cIsSelf = blocks[newPos.x, newPos.y] == trgBlock; // Ne koristi se jer bi za Jump trebalo da se to omoguci

        Debug.Log("CanNotMove: newPos = " + newPos + " => In Bounds: " + cInBounds + ", Has Floor: " + cHasFloor + ", Not Blockable: " + cNotBlockable);

        return cInBounds && cHasFloor && (cNotBlockable || cIsSelf);
    }

    public void InsertItemIntoBlock(Block block, Vector2Int itemDistanceFromBlock, bool copy = false)
    {
        // Da li je u dati blok moguce ubaciti Item
        if(!(block is IContainsItem)) {
            // ConsoleController.InstructionInterrupted();
            return;
        }
        
        Vector2Int pos;
        
        if(!FindPositionOfBlock(block, out pos)) {
            // ConsoleController.InstructionInterrupted();
            return;
        }
        
        Vector2Int itemPos = pos + itemDistanceFromBlock;

        // Ako Item postoji, uzeti ga
        if(blocks[itemPos.x, itemPos.y] != null && blocks[itemPos.x, itemPos.y] is IContainsItem && (blocks[itemPos.x, itemPos.y] as IContainsItem).HasItem())
        {
            Item item = (!copy) ? (blocks[itemPos.x, itemPos.y] as IContainsItem).TakeItem() : (blocks[itemPos.x, itemPos.y] as IContainsItem).CloneItem();
            (block as IContainsItem).InsertItem(item);
        }
        else {
            Debug.LogWarning("Insert instruction failed!");
            ConsoleController.InstructionInterrupted();
        }
    }

    // > IControllerToMap Implementation

    public void PlayerMove(Vector2Int dir)
    {
        // Debug.Log("MovePlayer(" + dir + ")");
        MoveBlock(player, dir, false);
    }

    public void PlayerJump()
    {
        // Debug.Log("PlayerJump");
        MoveBlock(player, InitPlayerPos - CurrentPlayerPos, true);
    }

    public void PlayerPickItem(Vector2Int dir)
    {
        // Debug.Log(">>>>> Pick Item");
        InsertItemIntoBlock(player, dir);
    }

    public void PlayerCopyItem(Vector2Int dir)
    {
        // Debug.Log(">>>>> Copy Item");
        InsertItemIntoBlock(player, dir, copy:true);
    }

    public void PlayerDropItem(Vector2Int dir)
    {
        // Debug.Log(">>>>> Drop Item");
        Vector2Int playerPos;
        if (!FindPositionOfBlock(player, out playerPos))
            return;

        Vector2Int trgPos = playerPos + dir;
        Block trgBlock = blocks[trgPos.x, trgPos.y];

        // Debug.LogWarning("DIR: " + trgPos.x + ", " + trgPos.y);
        if (trgBlock is IContainsItem) {
            Item item = player.TakeItem();
            (trgBlock as IContainsItem).InsertItem(item);
        }
        else {
            Debug.LogWarning("Drop instruction failed!");
        }
        
    }

    // public void InstantiatePlayer(Vector2Int pos)
    // {
    //     // Ako vec postoji
    //     if(player != null)
    //         GameObject.Destroy(player);

    //     // Kreiranje objekta, i povezivanje sa mapom
    //     player = GameObject.Instantiate(playerPrefab, this.transform).GetComponent<PlayerBlock>();
    //     player.gameObject.name = "Player";
    //     player.map = this;

    //     // Sredjivanje inicijalne pozicije
    //     blocks[pos.x, pos.y] = player;
    //     player.SetRealTargetPosition(new Vector2(pos.x, pos.y) * virtToRealPositionRatio);
    // }

    /* -- Private helpers -- */

    bool CheckMapBounds(Vector2Int pos)
    {
        return pos.x < mapSize.x && pos.x >= 0 && pos.y < mapSize.y && pos.y >= 0;
    }

    /* -- Public helpers -- */

    public Vector3 VirtToRealPosition(Vector2Int pos)
    {
        return new Vector3(pos.x * virtToRealPositionRatio, 0, pos.y * virtToRealPositionRatio);
    }

    public PlayerBlock GetPlayer()
    {
        return player;
    }
}

public interface IConsoleToMap
{
    void PlayerMove(Vector2Int dir);
    void PlayerJump();
    void PlayerPickItem(Vector2Int dir);
    void PlayerCopyItem(Vector2Int dir);
    void PlayerDropItem(Vector2Int dir);
}

public class MapData
{
    public Vector2Int size;
    public GameObject[,] floor;
    public GameObject[,] blocks;
    public GameObject[,] itemsConf;
    public string goal;
    public string goalExpression;
    public string mapName;
}