using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Tile Settings")] 
    public float outerSize;
    public float innerSize;
    public float height;
    public bool isFlatTopped;
    public Material material;

    private List<GameObject> _grids;

     private void Awake()
     { 
         _grids = new List<GameObject>();
     }

     private void Start()
     {
         outerSize = Hex.Radius;
         innerSize = outerSize - 0.01f;
     }
    // private void Start()
    // {
    //     LayoutGrid();
    // }

    public void LayoutGrid()
    {
        ClearGrid();

        var gridPositions = CombatSystem.instance.tileSystem.GetAllTilePos();
    
        foreach (var pos in gridPositions)
        {
            GameObject grid = new GameObject($"Grid {pos.x},{pos.y},{pos.z}",typeof(HexGridRenderer));
            grid.transform.position = Hex.Hex2World(pos);
            grid.layer = 5; //UI Layer

            HexGridRenderer hexGridRenderer = grid.GetComponent<HexGridRenderer>();
            hexGridRenderer.isFlatTopped = isFlatTopped;
            hexGridRenderer.outerSize = outerSize;
            hexGridRenderer.innerSize = innerSize;
            hexGridRenderer.height = height;

            hexGridRenderer.SetMaterial(material);
            hexGridRenderer.DrawMesh();
            
            grid.transform.SetParent(transform, true);
            
            _grids.Add(grid);
        }
    }

    private void ClearGrid()
    {
        if (_grids is null) return;
        if (_grids.Count == 0) return;
        
        foreach (var grid in _grids)
        {
            Destroy(grid);
        }

        _grids.Clear();
    }

    [ContextMenu("Show Grid")]
    private void ShowGridInEditMode()
    {
        
    }
}
