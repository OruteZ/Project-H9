using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatEditorViewer : MonoBehaviour
{
    private CombatMapEditor _combatMapEditor;
    private EditorMouseController _editorMouseController;
    
    public GameObject tileViewerObj;
    private readonly HashSet<HexTransform> _tileViewerObjs = new ();
    
    public GameObject spawnPointViewerObj;
    private readonly HashSet<HexTransform> _spawnPointViewerObjs = new ();

    public GameObject playerSpawnPointViewerObj;
    private HexTransform _playerSpawnPointViewerObj;
    
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
            bool isFound = _tileViewerObjs.Any(view => view.position == tile);

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

    private void OnSpawnPointsChanged()
    {
        //get spawn points
        var spawnPoints = _combatMapEditor.spawnPoints;
        
        //find all tile viewer objects not in spawn points and remove
        //compare by hexPosition
        var list = _spawnPointViewerObjs.ToList();
        foreach (var view in list)
        {
            bool isFound = 
                spawnPoints.Any(spawnPoint => view.position == spawnPoint);

            if (!isFound)
            {
                Destroy(view.gameObject);
                _spawnPointViewerObjs.Remove(view);
            }
        }
        
        //find all spawn points not in tile viewer objects and add
        foreach (Vector3Int spawnPoint in spawnPoints)
        {
            bool isFound = _spawnPointViewerObjs.Any(view => view.position == spawnPoint);

            if (!isFound)
            {
                var spawnPointViewer = Instantiate(spawnPointViewerObj, transform);
                spawnPointViewer.transform.position = Hex.Hex2World(spawnPoint);
                
                var spawnPointViewerComponent = spawnPointViewer.GetComponent<HexTransform>();
                spawnPointViewerComponent.position = spawnPoint;
                
                _spawnPointViewerObjs.Add(spawnPointViewerComponent);
            }
        }
        
        //find player spawn point
        var playerSpawnPoint = _combatMapEditor.playerSpawnPoint;
        
        //find player spawn point viewer object not in player spawn point and remove
        //compare by hexPosition
        if (_playerSpawnPointViewerObj != null)
        {
            if (_playerSpawnPointViewerObj.position != playerSpawnPoint)
            {
                Destroy(_playerSpawnPointViewerObj.gameObject);
                _playerSpawnPointViewerObj = null;
            }
        }
        
        //find player spawn point not in player spawn point viewer object and add
        if (_playerSpawnPointViewerObj == null)
        {
            GameObject playerSpawnPointViewer = Instantiate(playerSpawnPointViewerObj, transform);
            playerSpawnPointViewer.transform.position = Hex.Hex2World(playerSpawnPoint);
                
            _playerSpawnPointViewerObj = playerSpawnPointViewer.GetComponent<HexTransform>();
            _playerSpawnPointViewerObj.position = playerSpawnPoint;
        }
    }
    
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
        
        //Find CombatMapEditor
        _combatMapEditor = FindObjectOfType<CombatMapEditor>();
        if (_combatMapEditor == null)
        {
            Debug.LogError("CombatMapEditor not found");
            return;
        }
        _combatMapEditor.onPointsChanged.AddListener(OnSpawnPointsChanged);
        
        //cleat sets
        _tileViewerObjs.Clear();
        OnSelectedTileChanged();
    }
}