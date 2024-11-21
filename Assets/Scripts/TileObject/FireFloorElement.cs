using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFloorElement : TileObject
{
    public FireFloor fireFloorController { get; private set; }

    public override string[] GetArgs()
    {
        return null;
    }

    public override void SetArgs(string[] args) { }
    public void SetUp(Vector3Int hexPosition, FireFloor ff)
    {
        this.hexPosition = hexPosition;
        this.fireFloorController = ff;
        base.SetUp();
    }
    public void ForcedDestroy()
    {
        RemoveSelf();
    }
}
