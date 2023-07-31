using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "ScriptableObjects/EnemyDatabase", order = 1)]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] 
    private List<EnemyInfo> enemyInfos;
    
    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("EnemyTable");

        if (enemyInfos is null) enemyInfos = new List<EnemyInfo>();
        else enemyInfos.Clear();

        for (var i = 0; i < dataList.Count; i++)
        {
            var curData = new EnemyInfo
            {
                index = int.Parse(dataList[i][0]),
                nameIndex = int.Parse(dataList[i][1]),
                stat = new UnitStat
                {
                    maxHp = int.Parse(dataList[i][2]),
                    concentration = int.Parse(dataList[i][3]),
                    sightRange = int.Parse(dataList[i][4]),
                    speed = int.Parse(dataList[i][5]),
                    actionPoint = int.Parse(dataList[i][6]),
                    additionalHitRate = float.Parse(dataList[i][7]),
                    criticalChance = float.Parse(dataList[i][8]),
                    
                    //추가 데미지
                    revolverAdditionalDamage = 0,
                    repeaterAdditionalDamage = 0,
                    shotgunAdditionalDamage = 0,
                    
                    //추가 사거리
                    revolverAdditionalRange = 0,
                    repeaterAdditionalRange = 0,
                    shotgunAdditionalRange = 0,
                    
                    //크뎀
                    revolverCriticalDamage = float.Parse(dataList[i][9]),
                    repeaterCriticalDamage = float.Parse(dataList[i][9]),
                    shotgunCriticalDamage = float.Parse(dataList[i][9]),
                },
                weaponIndex = int.Parse(dataList[i][10]),
                btIndex = int.Parse(dataList[i][11]),
            };
            
            enemyInfos.Add(curData);
        }
    }

    public EnemyInfo GetInfo(int index)
    {
        foreach (var data in enemyInfos)
        {
            if (data.index == index) return data;
        }

        Debug.LogError("Wrong index");
        return new EnemyInfo();
    }
}

[System.Serializable]
public struct EnemyInfo
{
    public int index;
    public int nameIndex;
    
    [Header("UnitStat")]
    public UnitStat stat;
    
    public int weaponIndex;
    public int btIndex;
    // todo : Reward
}
