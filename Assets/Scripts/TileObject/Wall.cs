using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : TileObject
{
    [Header("Wall type")]
    public bool visible;
    public bool walkable;
    public bool rayThroughable;
    
    public override void SetTile(Tile t)
    {
        Debug.Log("Se tTile call");
        base.SetTile(t);

        t.walkable = walkable;
        t.visible = visible;
        t.rayThroughable = rayThroughable;
    }
}
