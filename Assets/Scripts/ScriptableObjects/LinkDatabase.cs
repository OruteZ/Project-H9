using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinkDatabase", menuName = "ScriptableObjects/LinkTable", order = 0)]
public class LinkDatabase : ScriptableObject
{
    [SerializeField] private List<LinkData> dataBase;
    public int Length() => dataBase.Count;
    
    [ContextMenu("Read CSV")]
    public void ReadCsv()
    {
        var dataList = FileRead.Read("LinkTable");
        if (dataList is null)
        {
            Debug.LogError("There is no LinkTable");
            return;
        }

        if (dataBase is null) dataBase = new List<LinkData>();
        else dataBase.Clear();
        
        foreach(var data in dataList)
        {
            var newData = new LinkData
            {
                index = int.Parse(data[(int)LinkColumn.Index]),
                groupNameIndex = int.Parse(data[(int)LinkColumn.GroupNameIndex]),
                combatEnemy = FileRead.ConvertStringToArray<int>(data[(int)LinkColumn.CombatEnemy]),
            };
            
            dataBase.Add(newData);
        }
    }

    public LinkData GetData(int index)
    {
        foreach (var d in dataBase)
        {
            if (d.index == index) return d;
        }

        Debug.LogError("There is no Link Data that has index " + index);
        return dataBase[0];
    }
}

[Serializable]
public struct LinkData
{
    public int index;
    public int groupNameIndex;
    public int[] combatEnemy;
}

internal enum LinkColumn
{
    Index,
    GroupNameIndex,
    CombatEnemy,
}