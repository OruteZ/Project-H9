using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetPlayerSpawn : IEditorCommand
{
    public Vector3Int beforePos;
    public CombatMapEditor editor;

    public SetPlayerSpawn()
    {
        editor = Object.FindObjectOfType<CombatMapEditor>();
    }
    
    public void Execute(IEnumerable<Tile> tiles)
    {
        beforePos = editor.playerSpawnPoint;
        if (tiles.Count() != 1)
        {
            Debug.LogError("Wrong selected tile count");
            return;
        }

        editor.playerSpawnPoint = tiles.First().hexPosition;
        editor.Save();
    }

    public void Undo()
    {
        editor.playerSpawnPoint = beforePos;
        editor.Save();
    }
}