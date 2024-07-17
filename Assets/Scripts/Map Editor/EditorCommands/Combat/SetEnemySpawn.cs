using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SetEnemySpawn : IEditorCommand
{
    private readonly CombatMapEditor _mapEditor;
    private readonly Stack<Vector3Int> _addedPositions = new ();

    public SetEnemySpawn()
    {
        _mapEditor = Object.FindObjectOfType<CombatMapEditor>();
    }
    
    public void Execute(IEnumerable<Tile> tiles)
    {
        var existingPoints = _mapEditor.spawnPoints;

        foreach (var tile in tiles)
        {
            var contains = existingPoints.Contains(tile.hexPosition);
            if (contains) continue;

            _addedPositions.Push(tile.hexPosition);
        }
        
        // if(_addedPositions.Count + existingPoints.Count > 
        //    _mapEditor.GetCurrentMaxSize())
        // {
        //     Debug.LogError("Too many enemy spawn points"
        //         + " Cur max Size" + _mapEditor.GetCurrentMaxSize()
        //         + " Cur size" + _addedPositions.Count
        //         + " Existing size" + existingPoints.Count);
        //     return;
        // }
        
        foreach (Vector3Int pos in _addedPositions)
        {
            _mapEditor.spawnPoints.Add(pos);
        }

        _mapEditor.Save();
    }

    public void Undo()
    {
        foreach (var pos in _addedPositions)
        {
            _mapEditor.spawnPoints.Remove(pos);
        }

        _mapEditor.Save();
    }
}