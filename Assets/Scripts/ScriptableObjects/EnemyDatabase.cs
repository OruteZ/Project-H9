using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "ScriptableObjects/EnemyDatabase", order = 1)]
public class EnemyDatabase : ScriptableObject
{
    private const int INDEX = 0;
    private const int NAME_INDEX = 1;
    private const int HP = 2;
    private const int CONCENTRATION = 3;
    private const int SIGHT_RANGE = 4;
    private const int SPEED = 5;
    private const int ACTION_POINT = 6;
    private const int ADDITIONAL_HIT_RATE = 7;
    private const int CRIT_CHANCE = 8;
    private const int CRIT_DAMAGE = 9;
    private const int WEAPON_INDEX = 10;
    private const int BT_INDEX = 11;
    private const int MODEL_NAME = 12;
    private const int REWARD_GOLD = 13;
    private const int REWARD_EXP = 14;
    
    [SerializeField] 
    private List<EnemyData> enemyInfos;
    
    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("EnemyTable");

        if (enemyInfos is null) enemyInfos = new List<EnemyData>();
        else enemyInfos.Clear();

        foreach (var data in dataList)
        {
            var curData = new EnemyData
            { 
                index = int.Parse(data[INDEX]),
                nameIndex = int.Parse(data[NAME_INDEX]),
                stat = new UnitStat(),
                weaponIndex = int.Parse(data[WEAPON_INDEX]),
                btIndex = int.Parse(data[BT_INDEX]),
                model = Resources.Load("Prefab/Units/" + data[MODEL_NAME]) as GameObject,
                
                rewardGold = int.Parse(data[REWARD_GOLD]),
                rewardExp = int.Parse(data[REWARD_EXP]),
            };

            curData.stat.original[(int)StatType.MaxHp] = int.Parse(data[HP]);
            curData.stat.original[(int)StatType.CurHp] = int.Parse(data[HP]);;
            curData.stat.original[(int)StatType.Concentration] = int.Parse(data[CONCENTRATION]);;
            curData.stat.original[(int)StatType.SightRange] = int.Parse(data[SIGHT_RANGE]);;
            curData.stat.original[(int)StatType.Speed] = int.Parse(data[SPEED]);;
            curData.stat.original[(int)StatType.MaxActionPoint] = int.Parse(data[ACTION_POINT]);
            curData.stat.original[(int)StatType.CurActionPoint] = int.Parse(data[ACTION_POINT]);
            curData.stat.original[(int)StatType.AdditionalHitRate] = int.Parse(data[ADDITIONAL_HIT_RATE]);
            curData.stat.original[(int)StatType.CriticalChance] = int.Parse(data[CRIT_CHANCE]);
            curData.stat.original[(int)StatType.RevolverAdditionalDamage] = 0;
            curData.stat.original[(int)StatType.RepeaterAdditionalDamage] = 0;
            curData.stat.original[(int)StatType.ShotgunAdditionalDamage] = 0;
            curData.stat.original[(int)StatType.RevolverAdditionalRange] = 0;
            curData.stat.original[(int)StatType.RepeaterAdditionalRange] = 0;
            curData.stat.original[(int)StatType.ShotgunAdditionalRange] = 0;
            curData.stat.original[(int)StatType.RevolverCriticalDamage] = int.Parse(data[CRIT_DAMAGE]);
            curData.stat.original[(int)StatType.RepeaterCriticalDamage] = int.Parse(data[CRIT_DAMAGE]);
            curData.stat.original[(int)StatType.ShotgunCriticalDamage] = int.Parse(data[CRIT_DAMAGE]);

            enemyInfos.Add(curData);
        }
    }

    public EnemyData GetInfo(int index)
    {
        foreach (var data in enemyInfos)
        {
            if (data.index == index) return data;
        }

        Debug.LogError("Wrong index");
        return new EnemyData();
    }
}

[System.Serializable]
public struct EnemyData
{
    public int index;
    public int nameIndex;
    public GameObject model;
    
    [Header("UnitStat")]
    public UnitStat stat;
    
    public int weaponIndex;
    public int btIndex;
    // todo : Reward
    public int rewardGold;
    public int rewardExp;
    public string rewardItem;
    public string rewardWeapon;
}

public struct UnitModelData
{
    public string name;
    public GameObject modelObject;
}
