using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetPlayerSpawn : IEditorCommand
{
    private Vector3Int _beforePos;
    private readonly CombatMapEditor _editor = Object.FindObjectOfType<CombatMapEditor>();

    public void Execute(IEnumerable<Tile> tiles)
    {
        _beforePos = _editor.playerSpawnPoint;
        if (tiles.Count() != 1)
        {
            Debug.LogError("Wrong selected tile count");
            return;
        }

        _editor.playerSpawnPoint = tiles.First().hexPosition;
        _editor.Save();
    }

    public void Undo()
    {
        _editor.playerSpawnPoint = _beforePos;
        _editor.Save();
    }
}