using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Tile Settings")] public float thickness;
    public float height;
    public bool isFlatTopped;
    public Material material;

    private List<GameObject> _grids;
    private List<GameObject> gridList => _grids ?? new List<GameObject>();

    private void Start()
    {
        LayoutGrid();
    }

    [ContextMenu("Layout Menu")]
    public void LayoutGrid()
    {
        ClearGrid();
        var outerSize = Hex.Radius;
        var innerSize = outerSize - thickness;
        
        var grids = Application.isPlaying ? FieldSystem.tileSystem.GetAllTiles() : 
            GetComponentsInChildren<Tile>().ToList();
       
    
        foreach (var grid in grids)
        {
            HexGridRenderer hexGridRenderer = grid.gameObject.AddComponent<HexGridRenderer>();
            hexGridRenderer.isFlatTopped = isFlatTopped;
            hexGridRenderer.outerSize = outerSize;
            hexGridRenderer.innerSize = innerSize;
            hexGridRenderer.height = height;

            hexGridRenderer.SetUp();
            hexGridRenderer.SetMaterial(material);
            hexGridRenderer.DrawMesh();
            
            grid.transform.SetParent(transform, true);
            gridList.Add(grid.gameObject);
        }
    }

    public void ClearGrid()
    {
        if (gridList is null) return;
        if (gridList.Count == 0) return;
        
        foreach (var grid in gridList)
        {
            Destroy(grid);
        }

        gridList.Clear();
    }
}
