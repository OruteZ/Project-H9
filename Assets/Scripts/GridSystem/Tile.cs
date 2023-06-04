using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector] public HexTransform hexTransform;

    public Vector3Int position
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    } 

    private MeshRenderer _meshRenderer;

    [Header("타일 속성")] public bool walkable;
    public bool visible;
    public bool rayThroughable;

    [Header("플레이어 시야")] [SerializeField] private bool _inSight;

    private Material _curEffect;

    public Material effect
    {
        get => _curEffect;
        set => _meshRenderer.material = value;
    }

    public List<TileObject> objects;
    protected void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void AddObject(TileObject u)
    {
        objects.Add(u);
    }

    public void RemoveObject(TileObject u)
    {
        objects.Remove(u);
    }
    private void ReloadEffect()
    {
        if (!visible) TileEffectManager.SetEffect(this, EffectType.Impossible);
        else TileEffectManager.SetEffect(this, EffectType.Normal);
    }

    public bool inSight
    {
        get => _inSight;
        set
        {
            _inSight = value;
            foreach (var obj in objects)
            { 
                obj.isVisible = value;
            }
            
            var unit = MainSystem.instance.unitSystem.GetUnit(position);
            if (unit is not null) unit.isVisible = value;
        }
    }

    
}