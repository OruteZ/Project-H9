using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditLinkInfo : IEditorCommand
{
    List<Link> _links;
    List<int> _linkIndexes;
    List<int> _combatMapIndexes;
    
    public void Execute(IEnumerable<Tile> tiles)
    {
        _links = new();
        
        //get all links that has same position with tiles
        var links = Object.FindObjectsOfType<Link>();
        foreach (var link in links)
        {
            bool samePos = tiles.Any(tile => link.hexPosition == tile.hexPosition);
            if(samePos) //if same position, add to list
            {
                _links.Add(link);
            }
        }
        
        //store link index and combat map index
        _linkIndexes = new List<int>();
        _combatMapIndexes = new List<int>();
        
        foreach (var link in _links)
        {
            _linkIndexes.Add(link.linkIndex);
            _combatMapIndexes.Add(link.combatMapIndex);
        }
        
        //find editor
        var worldEditor = Object.FindObjectOfType<WorldLinkInfoEditor>().GetComponent<WorldLinkInfoEditor>();
        worldEditor.SetEditCommand(this);
        worldEditor.ShowEditorUI(_links);
    }

    public void Undo()
    {
        //set back link index and combat map index
        for (int i = 0; i < _links.Count; i++)
        {
            _links[i].linkIndex = _linkIndexes[i];
            _links[i].combatMapIndex = _combatMapIndexes[i];
        }
    }
    
    public void ApplyIndexes(int linkIndex, int combatMapIndex)
    {
        
        //set link index and combat map index
        foreach (var link in _links)
        {
            link.linkIndex = linkIndex;
            FieldSystem.
                tileSystem.
                GetTile(link.hexPosition).
                combatStageIndex = combatMapIndex;
        }
        
        //find editor and save
        var worldEditor = Object.FindObjectOfType<WorldMapEditor>().GetComponent<WorldMapEditor>();
        worldEditor.SaveLink();
    }
}