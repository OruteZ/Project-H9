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
                stat = new UnitStat
                {
                    maxHp = int.Parse(data[HP]),
                    concentration = int.Parse(data[CONCENTRATION]),
                    sightRange = int.Parse(data[SIGHT_RANGE]),
                    speed = int.Parse(data[SPEED]),
                    actionPoint = int.Parse(data[ACTION_POINT]),
                    additionalHitRate = float.Parse(data[ADDITIONAL_HIT_RATE]),
                    criticalChance = float.Parse(data[CRIT_CHANCE]),
                    
                    //추가 데미지
                    revolverAdditionalDamage = 0,
                    repeaterAdditionalDamage = 0,
                    shotgunAdditionalDamage = 0,
                    
                    //추가 사거리
                    revolverAdditionalRange = 0,
                    repeaterAdditionalRange = 0,
                    shotgunAdditionalRange = 0,
                    
                    //크뎀
                    revolverCriticalDamage = float.Parse(data[CRIT_DAMAGE]),
                    repeaterCriticalDamage = float.Parse(data[CRIT_DAMAGE]),
                    shotgunCriticalDamage = float.Parse(data[CRIT_DAMAGE]),
                },
                weaponIndex = int.Parse(data[WEAPON_INDEX]),
                btIndex = int.Parse(data[BT_INDEX]),
                model = Resources.Load("Prefab/Units/" + data[MODEL_NAME]) as GameObject,
                
                rewardGold = int.Parse(data[REWARD_GOLD]),
                rewardExp = int.Parse(data[REWARD_EXP]),
            };
            
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
