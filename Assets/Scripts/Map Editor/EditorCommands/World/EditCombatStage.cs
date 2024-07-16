using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class EditCombatStage : IEditorCommand
{
    public WorldMapEditor editor = Object.FindObjectOfType<WorldMapEditor>();
    
    //stack to store the previous state
    private readonly List<TileCombatStageInfo> _previousState = new();
    private readonly List<TileCombatStageInfo> _currentState = new();
    
    public void Execute(IEnumerable<Tile> tiles)
    {
        var enumerable = tiles as Tile[] ?? tiles.ToArray();
        foreach(Tile tile in enumerable)
        {
            _previousState.Add(new TileCombatStageInfo
            {
                combatStageIndex = tile.combatStageIndex,
                hexPosition = tile.hexPosition
            });
        }
        
        foreach(Tile tile in enumerable)
        {
            // tile.combatStageIndex = editor.combatStageIndex;
            _currentState.Add(new TileCombatStageInfo
            {
                // combatStageIndex = editor.combatStageIndex,
                hexPosition = tile.hexPosition
            });
        }
        
        //find editor
        var worldEditor = Object.FindObjectOfType<WorldTileCombatIndexEditor>().
            GetComponent<WorldTileCombatIndexEditor>();
        worldEditor.SetEditCommand(this);
        worldEditor.ShowEditorUI(_currentState);
    }

    public void Undo()
    {
        foreach (TileCombatStageInfo tileCombatStageInfo in _previousState)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(tileCombatStageInfo.hexPosition);
            tile.combatStageIndex = tileCombatStageInfo.combatStageIndex;
        }
    }

    public void ApplyIndexes(int combatMapIndexInt)
    {
        // get world data
        // find all tile
        // set combatStageIndex to all tile
        foreach (TileCombatStageInfo tileCombatStageInfo in _currentState)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(tileCombatStageInfo.hexPosition);
            tile.combatStageIndex = combatMapIndexInt;
        }
        
        
        //find editor and save
        WorldMapEditor worldEditor = 
            Object
                .FindObjectOfType<WorldMapEditor>()
                .GetComponent<WorldMapEditor>();
        
        worldEditor.SaveData();
        
        //find EditorMouseController
        EditorMouseController editorMouseController = 
            Object
                .FindObjectOfType<EditorMouseController>()
                .GetComponent<EditorMouseController>();
        
        editorMouseController.onCommandExecuted.Invoke();
    }
}

[Serializable]
public struct TileCombatStageInfo
{
    public int combatStageIndex;
    [FormerlySerializedAs("tilePos")] public Vector3Int hexPosition;
}