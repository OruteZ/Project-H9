using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public abstract class TileObject : MonoBehaviour
{
    public TileObjectType objectType;
    
    [SerializeField] 
    private bool vision;
    
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
        InitRenderer();
    }

    protected virtual void InitRenderer()
    {
        TryGetComponent(out meshRenderer);
        meshRenderer ??= GetComponentInChildren<Renderer>();
    }

    public virtual void SetUp()
    {
        tile = FieldSystem.tileSystem.GetTile(hexPosition);
        if(tile == null) Debug.LogError("타일이 없는 곳으로 Tile Object 배치 : hexPosition = " + hexPosition);
        
        SetTile(tile);
        vision = IsVisible();
        //FieldSystem.tileSystem.AddTileObject(this);
    }

    protected virtual void SetTile(Tile t)
    {
        t.AddObject(this);
        tile = t;
    }

    public virtual void OnHexCollisionEnter(Unit other)
    {
        
    }
    
    public virtual void OnHexCollisionExit(Unit other)
    {
        
    }

    public virtual bool IsVisible()
    {
        return meshRenderer.enabled;
    }

    public virtual void SetVisible(bool value)
    {
        //if editor mode, value always true
        if (GameManager.instance.CompareState(GameState.EDITOR)) value = true;
        
        meshRenderer.enabled = value;
        vision = value;
    }

    protected virtual void RemoveSelf()
    {
        FieldSystem.tileSystem.DeleteTileObject(this);
        tile.RemoveObject(this);
        Destroy(gameObject);
    }

    public abstract string[] GetArgs();
    public abstract void SetArgs(string[] args);
}

public enum TileObjectType
{
    LINK,
    FOG_OF_WAR,
    TOWN,
    COVERABLE,
    DYNAMITE,
    HEINRICH_TRAP,
    NONE,
    TNT_BARREL,
    OIL_BARREL,
    FIRE_FLOOR,
    TRAP,
    BEER,
    COLD_WAVE_TRIGGER,
    BUSH,
    
}
