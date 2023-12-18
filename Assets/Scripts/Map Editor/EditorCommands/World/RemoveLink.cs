using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemoveLink : IEditorCommand
{
    //combat encounter prefab
    private readonly GameObject _combatEncounter;
    private readonly Transform _tileObjectParent;
    
    private struct LinkInfo
    {
        public int linkIndex;
        public int combatMapIndex;
        public Vector3Int pos;
    }
    
    //constructor
    public RemoveLink()
    {
        _combatEncounter = Resources.Load<GameObject>("Prefab/TileObjects/Combat Encounter");
        //null check
        if (_combatEncounter == null)
        {
            Debug.LogError("CombatEncounter prefab not found!");
        }

        _tileObjectParent = GameObject.Find("TileObjects").transform;
        //null check
        if (_tileObjectParent == null)
        {
            Debug.LogError("TileObjects parent not found!");
        }
    }
    
    //add a new field to store the link
    private readonly List<LinkInfo> _removedLinks = new ();

    public void Execute(IEnumerable<Tile> tiles)
    {
        //get all combat encounter objects
        var combatEncounters = Object.FindObjectsOfType<Link>();
        
        //if combat encounter objects found in tiles, remove it
        //compare by position
        foreach (var link in combatEncounters)
        {
            bool samePos = tiles.Any(tile => link.hexPosition == tile.hexPosition);
            if(samePos) //if same position, remove
            {
                Debug.Log("Remove link");
                
                //save link info
                LinkInfo linkInfo = new LinkInfo
                {
                    linkIndex = link.linkIndex,
                    combatMapIndex = link.combatMapIndex,
                    pos = link.hexPosition
                };
                _removedLinks.Add(linkInfo);
                
                //remove
                Object.DestroyImmediate(link.gameObject);
            }
        }
        
        //save link
        var worldEditor = Object.FindObjectOfType<WorldMapEditor>().GetComponent<WorldMapEditor>();
        worldEditor.SaveLink();
    }

    public void Undo()
    {
        // instantiate remove links
        foreach (var linkInfo in _removedLinks)
        {
            var linkObject = Object.Instantiate(_combatEncounter, _tileObjectParent).GetComponent<Link>();
            
            linkObject.hexPosition = linkInfo.pos;
            linkObject.GetComponent<Link>().linkIndex = linkInfo.linkIndex;
            linkObject.GetComponent<Link>().combatMapIndex = linkInfo.combatMapIndex;
            
            //save link
            var worldEditor = Object.FindObjectOfType<WorldMapEditor>().GetComponent<WorldMapEditor>();
            worldEditor.SaveLink();
        }
    }
}
