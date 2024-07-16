using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetPlayerSpawn : IEditorCommand
{
    public Vector3Int beforePos;
    public CombatMapEditor editor = Object.FindObjectOfType<CombatMapEditor>();

    public void Execute(IEnumerable<Tile> tiles)
    {
        beforePos = editor.playerSpawnPoint;
        IEnumerable<Tile> list = tiles.ToList();
        if (list.Count() != 1)
        {
            Debug.LogError("Wrong selected tile count");
            return;
        }

        editor.playerSpawnPoint = list.First().hexPosition;
        editor.Save();
    }

    public void Undo()
    {
        editor.playerSpawnPoint = beforePos;
        editor.Save();
    }
}