using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class TileObject : MonoBehaviour
{
    public HexTransform hexTransform;
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
}
