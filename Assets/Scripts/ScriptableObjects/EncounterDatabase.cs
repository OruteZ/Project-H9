using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterDatabase", menuName = "ScriptableObjects/EncounterDatabase", order = 1)]
public class EncounterDatabase : ScriptableObject
{
    [SerializeField]
    private List<EncounterData> encounterList;
    
    public EncounterData GetData(int index)
    {
        for (var i = 0; i < encounterList.Count; i++)
        {
            var data = encounterList[i];
            if (data.index == index) return data;
        }

        Debug.LogError("Wrong index");
        return new EncounterData();
    }

    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("EncounterTable");

        if (encounterList is null) encounterList = new List<EncounterData>();
        else encounterList.Clear();

        for (int i = 0; i < dataList.Count; i++)
        {
            var curData = new EncounterData
            {
                index = int.Parse(dataList[i][0]),
                groupNameIndex = int.Parse(dataList[i][1]),
                enemiesIndex = FileRead.ConvertStringToArray(dataList[i][2])
            };
            
            encounterList.Add(curData);
        }
    }
    
}

[System.Serializable]
public struct EncounterData
{
    public int index;
    public int groupNameIndex;
    public int[] enemiesIndex;
}
