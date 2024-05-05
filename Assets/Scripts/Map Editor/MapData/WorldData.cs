using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "World Obj Data", menuName = "WorldObjData", order = 0)]
public class WorldData : ScriptableObject
{
    public Vector3Int playerPosition;
    
    [SerializeField] 
    public List<LinkObjectData> links = new (); 

    public List<WorldFlags> flags;

    public HashSet<Vector3Int> discoveredWorldTileSet;

    public int worldTurn;
}

[Serializable]
public struct LinkObjectData
{
    public Vector3Int pos;

    public int linkIndex;
    public int combatMapIndex;
    public string modelName;
}

public enum WorldFlags
{
    
}