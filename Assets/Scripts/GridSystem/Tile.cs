using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector] public HexTransform hexTransform;

    public Vector3Int Position => hexTransform.position;

    private List<TileObject> _objects;
    private MeshRenderer _meshRenderer;

    [Header("타일 속성")] public bool walkable;
    public bool visible;
    public bool rayThroughable;

    [Header("플레이어 시야")] [SerializeField] private bool inSight;

    private Material _curEffect;

    public Material Effect
    {
        get => _curEffect;
        set => _meshRenderer.material = value;
    }

    protected void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _objects = new List<TileObject>();
    }

    public void AddObject(TileObject u)
    {
        _objects.Add(u);
    }

    private void ReloadEffect()
    {
        if(!inSight) TileEffector.SetEffect(this, EffectType.Invisible);
        else if(!visible) TileEffector.SetEffect(this, EffectType.Impossible);
        else TileEffector.SetEffect(this, EffectType.Normal);
    }

    public bool InSight
    {
        get => inSight;
        set
        {
            inSight = value;
            if (value)
            {
                foreach (var obj in _objects)
                {
                    obj.IsVisible = true;
                }
            }
            
            var unit = CombatManager.Instance.unitSystem.GetUnit(Position);
            if (unit != null) unit.IsVisible = value;

            ReloadEffect();
        }
    }
}