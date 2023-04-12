using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexTransform))]
public class Tile : MonoBehaviour
{
    [HideInInspector]
    public HexTransform hexTransform;

    // ReSharper disable once InconsistentNaming
    public Vector3Int position => hexTransform.position;

    private List<TileObject> objects;
    
    [Header("차후 제거할 요소들")]
    private MeshRenderer _meshRenderer;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blackMaterial;

    [Header("타일 속성")]
    public bool walkable;
    public bool visible;
    public bool rayThroughable;

    private GameObject _curEffect;
    public GameObject Effect
    {
        get => _curEffect;
        set
        {
            if (_curEffect != null)
            {
                Destroy(_curEffect);
            }
            
            Instantiate(value, transform);
            _curEffect = Effect;
        }
    }
    protected void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        _meshRenderer = GetComponent<MeshRenderer>();
        objects = new List<TileObject>();
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
        objects.Add(u);
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