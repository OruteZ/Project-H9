using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class TileObject : MonoBehaviour
{
    public HexTransform hexTransform;

    public Vector3Int Position
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    } 
    protected Tile tile;
    
    protected void Init()
    {
        hexTransform = GetComponent<HexTransform>();
    }

    public void SetTile(Tile t)
    {
        t.AddObject(this);
        tile = t;

        hexTransform.position = t.hexTransform.position;
    }
    
    public static void Spawn(TileSystem tileSystem, Vector3Int pos, GameObject obj)
    {
        var tile = tileSystem.GetTile(pos);
        if (tile == null) return;

        var gObject = Instantiate(obj, Hex.Hex2World(pos), Quaternion.identity);
        gObject.GetComponent<TileObject>().Init();
        gObject.GetComponent<TileObject>().SetTile(tile);
    }
}
