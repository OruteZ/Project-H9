using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldEditorViewer : MonoBehaviour
{
    #region GAMEOBJECTS
    private WorldMapEditor _worldMapEditor;
    private EditorMouseController _editorMouseController;
    
    public GameObject tileViewerObj;
    private readonly HashSet<HexTransform> _tileViewerObjs = new ();

    private void OnSelectedTileChanged()
    {
        // get selected tiles positions by set
        var selectedTilesPositions = new HashSet<Vector3Int>();
        foreach (var tile in _editorMouseController.GetSelectedTiles())
        {
            selectedTilesPositions.Add(tile.hexPosition);
        }
        
        // find all tile viewer objects not in selected tiles and remove
        // compare by hexPosition
        var list = _tileViewerObjs.ToList();
        foreach (var view in list)
        {
            bool isFound = 
                selectedTilesPositions.Any(selectedTilePosition => view.position == selectedTilePosition);

            if (!isFound)
            {
                Destroy(view.gameObject);
                _tileViewerObjs.Remove(view);
            }
        }

        // find all selected tiles not in tile viewer objects and add
        foreach (var tile in selectedTilesPositions)
        {
            bool isFound = false;
            foreach (var view in _tileViewerObjs)
            {
                if (view.position == tile)
                {
                    isFound = true;
                    break;
                }
            }
            
            if (!isFound)
            {
                var tileViewer = Instantiate(tileViewerObj, transform);
                tileViewer.transform.position = Hex.Hex2World(tile);
                
                var tileViewerComponent = tileViewer.GetComponent<HexTransform>();
                tileViewerComponent.position = tile;
                
                _tileViewerObjs.Add(tileViewerComponent);
            }
        }
    }
    
    #endregion
    
    #region UI

    private Canvas _canvas;
    
    #endregion
    
    private void Awake()
    {
        //Find Editor_MouseController
        _editorMouseController = FindObjectOfType<EditorMouseController>();
        if (_editorMouseController == null)
        {
            Debug.LogError("Editor_MouseController not found");
            return;
        }
        _editorMouseController.onSelectedTileChanged.AddListener(OnSelectedTileChanged);
        
        //cleat sets
        _tileViewerObjs.Clear();
        OnSelectedTileChanged();
    }
}