using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class TileObject : MonoBehaviour
{
    public HexTransform hexTransform;
    public MeshRenderer meshRenderer;

    protected Tile tile;
    public Vector3Int hexPosition
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    } 
    
    public static void Spawn(TileSystem tileSystem, Vector3Int pos, GameObject obj)
    {
        var tile = tileSystem.GetTile(pos);
        if (tile == null) return;
    
        var tileObj = Instantiate(obj, Hex.Hex2World(pos), Quaternion.identity).GetComponent<TileObject>();
        tileObj.hexPosition = pos;
        tileObj.SetUp();
    }

    private void Awake()
    {
        hexTransform ??= GetComponent<HexTransform>();
        meshRenderer ??= GetComponent<MeshRenderer>();
    }
    
    public void SetUp()
    {
        tile = FieldSystem.tileSystem.GetTile(hexPosition);
        if(tile == null) Debug.LogError("타일이 없는 곳으로 Tile Object 배치");
        
        SetTile(tile);
    }

    public void SetTile(Tile t)
    {
        t.AddObject(this);
        tile = t;
    }

    public virtual void OnCollision(Unit other)
    {
        
    }

    [SerializeField] private bool vision;
    public bool IsVisible()
    {
        return meshRenderer.enabled;
    }

    public void SetVisible(bool value)
    {
        meshRenderer.enabled = value;
        vision = value;
    }

    protected virtual void RemoveSelf()
    {
        tile.RemoveObject(this);
        Destroy(gameObject);
    }
}
