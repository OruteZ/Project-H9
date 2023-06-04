using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class TileObject : MonoBehaviour
{
    public HexTransform hexTransform;
    public MeshRenderer meshRenderer;

    public Vector3Int position
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    } 
    protected Tile tile;

    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    public void Init()
    {
        tile = MainSystem.instance.tileSystem.GetTile(position);
        if(tile == null) Debug.LogError("타일이 없는 곳으로 Tile Object 배치");
        
        SetTile(tile);
    }

    public void SetTile(Tile t)
    {
        t.AddObject(this);
        tile = t;
    }

    public virtual void OnCollision(Unit other)
    { }
    
    public static void Spawn(TileSystem tileSystem, Vector3Int pos, GameObject obj)
    {
        var tile = tileSystem.GetTile(pos);
        if (tile == null) return;

        var tileObj = Instantiate(obj, Hex.Hex2World(pos), Quaternion.identity).GetComponent<TileObject>();
        tileObj.position = pos;
    }

    [SerializeField] private bool vision;
    public bool isVisible
    {
        get => meshRenderer.enabled;
        set
        {
            meshRenderer.enabled = value;
            vision = value;
        }
    }

}
