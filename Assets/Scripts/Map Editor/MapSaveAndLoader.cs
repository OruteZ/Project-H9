using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapSaveAndLoader : MonoBehaviour
{
    public CombatStageData saveData;
    // public ScriptableObject TileObjectDatabase;

    public GameObject tilePrefab;
    public HexGridLayout layout;
    
    [ContextMenu("Save Current map")]
    public void SaveMap()
    {
        var envParent = GameObject.Find("Environments");
        var tileParent = GameObject.Find("Tiles");
        var tileObjParent = GameObject.Find("TileObjects");

        if (envParent is null)
        {
            Debug.LogError("Cant find 'Environments'");
            return;
        }
        
        if (tileParent is null)
        {
            Debug.LogError("Cant find 'Tiles'");
            return;
        }

        if (tileObjParent is null)
        {
            Debug.LogError("Cant find 'TileObjects'");
            return;
        }

        var tiles = tileParent.GetComponentsInChildren<Tile>();
        var tileList = new TileData[tiles.Length];
        for (var i = 0; i < tiles.Length; i++)
        {
            tileList[i]= new TileData(tiles[i]);
        }

        var objects = tileObjParent.GetComponentsInChildren<TileObject>();
        var tileObjList = new TileObjectData[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            tileObjList[i] = new TileObjectData(objects[i]);
        }
        
        var envDataList = new EnvironmentData[envParent.transform.childCount];
        for (int i = 0; i < envParent.transform.childCount; i++)
        {
            envDataList[i] = new EnvironmentData(envParent.transform.GetChild(i).gameObject);
        }
        
        saveData.SetTileDataList(tileList.ToList());
        saveData.SetTileObjectDataList(tileObjList.ToList());
        saveData.SetEnvDataList(envDataList.ToList());
    }

    [ContextMenu("Load Current map")]
    public void LoadMap()
    {
        GameObject envParent = GameObject.Find("Environments");
        GameObject tileParent = GameObject.Find("Tiles");
        GameObject tileObjParent = GameObject.Find("TileObjects");

        if (envParent is null)
        {
            Debug.LogError("Cant find 'Environments'");
            return;
        }
        
        if (tileParent is null)
        {
            Debug.LogError("Cant find 'Tiles'");
            return;
        }

        if (tileObjParent is null)
        {
            Debug.LogError("Cant find 'TileObjects'");
            return;
        }
        
        //Clear tiles
        var tiles = tileParent.GetComponentsInChildren<Tile>();
        foreach (Tile tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        
        //Clear tile objects
        var objects = tileObjParent.GetComponentsInChildren<TileObject>();
        foreach (TileObject tileObj in objects)
        {
            DestroyImmediate(tileObj.gameObject);
        }
        
        //Clear environments
        var envs = envParent.GetComponentsInChildren<HexTransform>();
        foreach (HexTransform env in envs)
        {
            DestroyImmediate(env.gameObject);
        }
        
        
        //instantiate tiles
        foreach (TileData tileData in saveData.GetTileDataList())
        {
            Tile tile = Instantiate(tilePrefab, tileParent.transform).GetComponent<Tile>();
            tile.visible = tileData.visible;
            tile.rayThroughable = tileData.rayThroughable;
            tile.gridVisible = tileData.gridVisible;
            tile.walkable = tileData.walkable;
            
            //set hexPosition
            tile.hexPosition = tileData.hexPosition;
            
            //set name
            tile.name = "Tile : " + tileData.hexPosition;
        }
        layout.LayoutGrid();
        
        //instantiate tile objects
        foreach (TileObjectData tileObjectData in saveData.GetTileObjectDataList())
        {
            if (tileObjectData.prefab is null)
            {
                Debug.LogError("Prefab is null");
                continue;
            }

            if (Instantiate(tileObjectData.prefab).TryGetComponent(out TileObject tileObject) is false)
            {
                Debug.LogError("TileObject도 아닌걸 왜 여기에 집어넣었지?");
                Debug.LogError(tileObjectData.prefab.name);
                continue;
            }
            
            tileObject.transform.parent = tileObjParent.transform;
            tileObject.hexPosition = tileObjectData.hexPosition;
            tileObject.gameObject.transform.position += tileObjectData.positionOffset;
            tileObject.gameObject.transform.localRotation = Quaternion.Euler(0, tileObjectData.rotation, 0);
            tileObject.SetArgs(tileObjectData.arguments);
            // tileObject.SetUp();
        }
        
        //instantiate environments
        foreach (EnvironmentData envData in saveData.GetEnvDataList())
        {
            GameObject   envObj = new GameObject();
            MeshRenderer meshRenderer = envObj.AddComponent<MeshRenderer>();
            MeshFilter   meshFilter = envObj.AddComponent<MeshFilter>();
            envObj.transform.parent = envParent.transform;

            //paste data
            envObj.transform.position = new Vector3(0, envData.height, 0);
            
            if (envData.tr)
                envObj.transform.localEulerAngles = envData.threeRotation;
            else
                envObj.transform.localRotation = Quaternion.Euler(0, envData.rotation, 0);
            
            envObj.transform.localScale = envData.scale;
            meshRenderer.material = envData.material;
            meshFilter.mesh = envData.mesh;
            
            //if hexPositioned, add HexTransform
            if (envData.hexPositioned)
            {
                HexTransform hexTransform = envObj.AddComponent<HexTransform>();
                hexTransform.position = envData.hexPosition;
            }
            else
            {
                envObj.transform.position = envData.position;
            }
        }
    }

    public CombatStageData GetData()
    {
        return saveData;
    }

    private void Awake()
    {
        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            saveData = GameManager.instance.GetStageData();
            LoadMap();
        }
    }
}
