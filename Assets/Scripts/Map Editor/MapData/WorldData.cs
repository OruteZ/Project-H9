using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "World Obj Data", menuName = "WorldObjData", order = 0)]
public class WorldData : ScriptableObject
{
    public Vector3Int playerPosition;
    
    [SerializeField] 
    public List<LinkObjectData> links = new (); 

    public List<WorldFlags> flags;

    public HashSet<Vector3Int> discoveredWorldTileSet;

    public int worldTurn;
    
    public bool TryAddLink(Vector3Int pos, float rotation, int linkIndex, int combatMapIndex = 1, GameObject model = null, bool isRepeatable = false)
    {
        if (model == null)
        {
            var linkDB = Resources.Load<LinkDatabase>("DataBase/LinkDatabase");
            model = linkDB.GetData(linkIndex).model;
        }
        
        if (links.Exists(x => x.pos == pos))
            return false;

        links.Add(new LinkObjectData
        {
            pos = pos,
            rotation = rotation,
            linkIndex = linkIndex,
            combatMapIndex = combatMapIndex,
            model = model,
            isRepeatable = isRepeatable
        });

        return true;
    }
}

[Serializable]
public struct LinkObjectData
{
    public Vector3Int pos;
    public float rotation;

    public int linkIndex;
    public int combatMapIndex;
    public GameObject model;
    public bool isRepeatable;
}

public enum WorldFlags
{
    
}