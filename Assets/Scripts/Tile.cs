using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector]
    public HexTransform hexTransform;

    private MeshRenderer _meshRenderer;
    
    private List<TileObject> _units;
    
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blackMaterial;

    public bool walkable;
    public bool passable;
    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _units = new List<TileObject>();
    }

    private void Start()
    {
        _meshRenderer.material = normalMaterial;
    }

    private void Update()
    {
        if (walkable == false)
        {
            Black = true;
        }
    }

    public void AddUnit(TileObject u)
    {
        _units.Add(u);
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