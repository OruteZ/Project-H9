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
        var outerSize = Hex.Radius + thickness;
        var innerSize = Hex.Radius - thickness;
        
        var grids = Application.isPlaying ? FieldSystem.tileSystem.GetAllTiles() : 
            GetComponentsInChildren<Tile>().ToList();
       
    
        foreach (var grid in grids)
        {
            if (grid.gridVisible is false)
            {
                grid.GetComponent<MeshRenderer>().enabled = false;
                continue;
            }
            
            HexGridRenderer hexGridRenderer = grid.gameObject.GetComponent<HexGridRenderer>();
            if (hexGridRenderer is null) hexGridRenderer = grid.gameObject.AddComponent<HexGridRenderer>();
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


    public Transform editor;
    [ContextMenu("EditorLayout Menu")]
    public void EditorLayoutGrid()
    {
        foreach (var grid in gridList)
        {
            DestroyImmediate(grid);
        }
        gridList.Clear();
        
        var outerSize = Hex.Radius;
        var innerSize = outerSize - thickness;
        
        var grids = Hex.GetCircleGridList(20, Hex.zero);
       
    
        foreach (var grid in grids)
        {
            HexGridRenderer hexGridRenderer = new GameObject($"{grid}").AddComponent<HexGridRenderer>();
            hexGridRenderer.transform.position = Hex.Hex2World(grid);
            
            hexGridRenderer.isFlatTopped = isFlatTopped;
            hexGridRenderer.outerSize = outerSize;
            hexGridRenderer.innerSize = innerSize;
            hexGridRenderer.height = height;

            hexGridRenderer.SetUp();
            hexGridRenderer.SetMaterial(material);
            hexGridRenderer.DrawMesh();
            
            hexGridRenderer.transform.SetParent(editor, true);
            gridList.Add(hexGridRenderer.gameObject);
        }
    }
}
