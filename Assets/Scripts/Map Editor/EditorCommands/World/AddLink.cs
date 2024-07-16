using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddLink : IEditorCommand
{
    //combat encounter prefab
    private readonly GameObject _combatEncounter;
    private readonly Transform _tileObjectParent;
    
    private readonly List<GameObject> _addedObjects = new List<GameObject>();

    public AddLink()
    {
        _combatEncounter = Resources.Load<GameObject>("Prefab/TileObjects/Combat Encounter");
        //null check
        if (_combatEncounter == null)
        {
            Debug.LogError("CombatEncounter prefab not found!");
        }
        
        _tileObjectParent = GameObject.Find("TileObjects").transform;
    }

    public void Execute(IEnumerable<Tile> tiles)
    {
        //instantiate combat encounter object every tiles
        foreach (var tile in tiles)
        {
            //if link that has same position with tile already exist, skip
            var links = Object.FindObjectsOfType<Link>();
            bool hasSamePos = links.Any(link => link.hexPosition == tile.hexPosition);
            if (hasSamePos) continue;
            

            var obj = Object.Instantiate
                (_combatEncounter, tile.transform.position, Quaternion.identity, _tileObjectParent);
            
            //getcomponent and set link index
            var combatEncounterScript = obj.GetComponent<Link>();
            combatEncounterScript.hexPosition = tile.hexPosition;
            combatEncounterScript.SetUp();
            
            //add to list
            _addedObjects.Add(obj);
        }
        
        //save link
        var worldEditor = Object.FindObjectOfType<WorldMapEditor>().GetComponent<WorldMapEditor>();
        worldEditor.SaveData();
    }

    public void Undo()
    {
        //remove added object
        foreach (var obj in _addedObjects)
        {
            Object.DestroyImmediate(obj);
        }
        
        //save link
        var worldEditor = Object.FindObjectOfType<WorldMapEditor>().GetComponent<WorldMapEditor>();
        worldEditor.SaveData();
    }
}