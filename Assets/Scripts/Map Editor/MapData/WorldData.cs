using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "World Obj Data", menuName = "WorldObjData", order = 0)]
public class WorldData : ScriptableObject
{
    public Vector3Int playerPosition;
    
    [SerializeField] 
    public List<LinkObjectData> links = new (); 

    public List<WorldFlags> flags;

    [SerializeField]
    private List<Vector3Int> discoveredWorldTileSet = new ();
    
    public List<TileCombatStageInfo> specificCombatIndexedTiles = new ();

    public int worldTurn;
    
    public static void SaveChangesToScriptableObject(ScriptableObject obj)
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
        #endif
    }
    
    public bool TryAddLink(Vector3Int pos, float rotation, int linkIndex, int combatMapIndex = 1, GameObject model = null, bool isRepeatable = false)
    {
        if (model == null)
        {
            LinkDatabase linkDB = Resources.Load<LinkDatabase>("DataBase/LinkDatabase");
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

    public void RemoveLink(Vector3Int hexPosition, int linkIndex)
    {
        links.RemoveAll(
            match:
            x => 
                x.pos == hexPosition &&
                x.linkIndex == linkIndex
            );
    }
    

    public bool FindDiscovered(Vector3Int tilePos, out int index)
    {
        Vector3IntCompare comparer = new Vector3IntCompare();
        
        index = discoveredWorldTileSet.BinarySearch(tilePos, comparer);
        return index >= 0;
    }
    
    public bool TryAddDiscovered(Vector3Int pos)
    {
        //find binary search
        if(FindDiscovered(pos, out int index))
            return false;
        
        //if not found, add
        //binary search return negative value
        discoveredWorldTileSet.Insert(~index, pos);
        return true;
    }
    
    public void RemoveDiscovered(Vector3Int pos)
    {
        //find binary search
        if(FindDiscovered(pos, out int index) is false) return;
        
        //if found, remove
        if (index >= 0)
        {
            discoveredWorldTileSet.RemoveAt(index);
        }
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