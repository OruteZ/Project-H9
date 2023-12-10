using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public abstract class TileObject : MonoBehaviour
{
    //for debug
    [SerializeField] 
    private bool vision;
    
    public int objectID;
    
    public HexTransform hexTransform;
    public Renderer meshRenderer;

    protected Tile tile;
    public Vector3Int hexPosition
    {
        get
        {
            if (hexTransform is null)
            {
                TryGetComponent(out hexTransform);
            }
            
            return hexTransform.position;
        }
        set
        {
            if (hexTransform is null)
            {
                TryGetComponent(out hexTransform);
            } 
            hexTransform.position = value;
        }
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
        //meshRenderer.enabled = GameManager.instance.CompareState(GameState.World);
        vision = IsVisible();
    }

    protected virtual void SetTile(Tile t)
    {
        t.AddObject(this);
        tile = t;
    }

    public virtual void OnCollision(Unit other)
    {
        
    }

    public bool IsVisible()
    {
        return meshRenderer.enabled;
    }

    public virtual void SetVisible(bool value)
    {
        meshRenderer.enabled = value;
        vision = value;
    }

    protected virtual void RemoveSelf()
    {
        tile.RemoveObject(this);
        Destroy(gameObject);
    }

    public abstract string[] GetArgs();
    public abstract void SetArgs(string[] args);

    [ContextMenu("Set MeshRenderer")]
    public void SetMesh()
    {
        hexTransform = GetComponent<HexTransform>();
        if (TryGetComponent(out meshRenderer) is false)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }
}
