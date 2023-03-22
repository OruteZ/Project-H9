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
    
    private List<Unit> _units;
    
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blackMaterial;

    public bool reachable;
    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _units = new List<Unit>();
    }

    private void Start()
    {
        _meshRenderer.material = normalMaterial;
    }

    private void Update()
    {
        if (reachable == false)
        {
            Black = true;
        }
    }

    public void AddUnit(Unit u)
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