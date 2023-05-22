using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector]
    public HexTransform hexTransform;

    public Vector3Int Position => hexTransform.position;

    private List<TileObject> _objects;
    private MeshRenderer _meshRenderer;

    [Header("타일 속성")]
    public bool walkable;
    public bool visible;
    public bool rayThroughable;

    [Header("플레이어 시야")]
    public bool inSight;

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
}