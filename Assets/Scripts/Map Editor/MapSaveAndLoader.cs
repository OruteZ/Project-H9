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
        saveData.tileData = new TileData[tiles.Length];
        for (var i = 0; i < tiles.Length; i++)
        {
            saveData.tileData[i]= new TileData(tiles[i]);
        }

        var objects = tileObjParent.GetComponentsInChildren<TileObject>();
        saveData.tileObjectData = new TileObjectData[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            saveData.tileObjectData[i] = new TileObjectData(objects[i]);
        }
        
        saveData.envData = new EnvironmentData[envParent.transform.childCount];
        for (int i = 0; i < envParent.transform.childCount; i++)
        {
            saveData.envData[i] = new EnvironmentData(envParent.transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("Load Current map")]
    public void LoadMap()
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
        
        //Clear tiles
        var tiles = tileParent.GetComponentsInChildren<Tile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            DestroyImmediate(tiles[i].gameObject);
        }
        
        //Clear tile objects
        var objects = tileObjParent.GetComponentsInChildren<TileObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            DestroyImmediate(objects[i].gameObject);
        }
        
        //Clear environments
        var envs = envParent.GetComponentsInChildren<HexTransform>();
        for (int i = 0; i < envs.Length; i++)
        {
            DestroyImmediate(envs[i].gameObject);
        }
        
        
        //instantiate tiles
        foreach (var tileData in saveData.tileData)
        {
            var tile = Instantiate(tilePrefab, tileParent.transform).GetComponent<Tile>();
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
        // foreach (var tileObjectData in saveData.tileObjectData)
        {
            // var tileObject = Instantiate(tileObjectPrefabs[tileObjectData.id], tileObjParent.transform).GetComponent<TileObject>();
            // tileObject.hexPosition = tileObjectData.hexPosition;
            // tileObject.gameObject.transform.localRotation = Quaternion.Euler(0, tileObjectData.rotation, 0);
            // tileObject.SetArgs(tileObjectData.arguments);
            // tileObject.SetUp();
        }
        
        //instantiate environments
        foreach (var envData in saveData.envData)
        {
            var envObj = new GameObject();
            var meshRenderer = envObj.AddComponent<MeshRenderer>();
            var meshFilter = envObj.AddComponent<MeshFilter>();
            envObj.transform.parent = envParent.transform;

            //paste data
            envObj.transform.position = new Vector3(0, envData.height, 0);
            envObj.transform.localRotation = Quaternion.Euler(0, envData.rotation, 0);
            envObj.transform.localScale = envData.scale;
            meshRenderer.material = envData.material;
            meshFilter.mesh = envData.mesh;
            
            //if hexPositioned, add HexTransform
            if (envData.hexPositioned)
            {
                var hexTransform = envObj.AddComponent<HexTransform>();
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
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            saveData = GameManager.instance.GetStageData();
            LoadMap();
        }
    }
}
