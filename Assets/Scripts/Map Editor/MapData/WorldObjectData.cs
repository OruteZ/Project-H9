using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "World Obj Data", menuName = "WorldObjData", order = 0)]
public class WorldObjectData : ScriptableObject
{
    [SerializeField] 
    public List<LinkData> links = new ();
}

[Serializable]
public struct LinkData
{
    public Vector3Int pos;

    public int linkIndex;
    public int combatMapIndex;
    public string modelName;
}