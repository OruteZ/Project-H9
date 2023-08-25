using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaveAndLoader : MonoBehaviour
{
    public MapData saveData;
    
    [ContextMenu("Save Current map")]
    public void SaveMap()
    {
        var envParent = GameObject.Find("Environments");
        var tileParent = GameObject.Find("Tiles");
        var objParent = GameObject.Find("TileObjects");

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

        if (objParent is null)
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

        var objects = objParent.GetComponentsInChildren<TileObject>();
        saveData.objectData = new ObjectMapData[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            saveData.objectData[i] = new ObjectMapData(objects[i]);
        }
    }

    public void LoadMap()
    {
        
    }
}
