using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSerializer : MonoBehaviour
{
    public Transform tileParent;
    
    [ContextMenu("Serialize")]  
    public void Serialize()
    {
        var tiles = tileParent.GetComponentsInChildren<Tile>();
        var tileData = new TileData[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            tileData[i] = new TileData(tiles[i]);
        }

        string json = JsonUtility.ToJson(new TileDataList(tileData), true);
        System.IO.File.WriteAllText("Assets/Resources/TileData/TileData.json", json);
    }
}

public class TileDataList
{
    public TileData[] tileData;
    
    public TileDataList(TileData[] tileData)
    {
        this.tileData = tileData;
    }
}
