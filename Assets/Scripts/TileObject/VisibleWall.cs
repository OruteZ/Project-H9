using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleWall : TileObject
{
    public override void SetTile(Tile t)
    {
        Debug.Log("Se tTile call");
        base.SetTile(t);

        t.walkable = false;
        t.visible = true;
        t.rayThroughable = false;
    }

    public override string[] GetArgs()
    {
        throw new System.NotImplementedException();
    }

    public override void SetArgs(string[] args)
    {
        throw new System.NotImplementedException();
    }
}
