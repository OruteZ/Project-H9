using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector]
    public HexTransform hexTransform;
    
    protected List<TileObject> _objects;
    
    [Header("차후 제거할 요소들")]
    private MeshRenderer _meshRenderer;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blackMaterial;

    [Header("타일 속성")]
    public bool walkable;
    public bool visible;
    public bool rayThroughable;
    protected void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _objects = new List<TileObject>();
    }

    protected void Start()
    {
        _meshRenderer.material = normalMaterial;
    }

    protected void Update()
    {
        if (walkable == false)
        {
            Black = true;
        }
    }

    public void AddObject(TileObject u)
    {
        _objects.Add(u);
    }
    
    public bool Highlight
    {
        get => _meshRenderer.material == highlightedMaterial;
        set =>  _meshRenderer.material = value ? highlightedMaterial : normalMaterial;
    }

    public bool Black
    {
        get => _meshRenderer.material == blackMaterial;
        set => _meshRenderer.material = value ? blackMaterial : normalMaterial;
    }
}