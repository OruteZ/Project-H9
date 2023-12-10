using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveEnemySpawn : IEditorCommand
{
    private readonly CombatMapEditor _mapEditor;
    private readonly Stack<Vector3Int> _removedPositions = new ();

    public RemoveEnemySpawn()
    {
        _mapEditor = Object.FindObjectOfType<CombatMapEditor>();
    }
    
    public void Execute(IEnumerable<Tile> tiles)
    {
        var existingPoints = _mapEditor.spawnPoints;

        foreach (var tile in tiles)
        {
            var contains = existingPoints.Contains(tile.hexPosition);
            if (contains is false) continue;

            _removedPositions.Push(tile.hexPosition);
            _mapEditor.spawnPoints.Remove(tile.hexPosition);
        }

        _mapEditor.Save();
    }

    public void Undo()
    {
        foreach (var pos in _removedPositions)
        {
            _mapEditor.spawnPoints.Add(pos);
        }

        _mapEditor.Save();
    }
}