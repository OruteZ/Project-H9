using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "ScriptableObjects/EnemyDatabase", order = 1)]
public class EnemyDatabase : ScriptableObject
{
    #region COLUMN
    private enum Col
    {
        Index,
        EnemyName,
        ModelFile,
        Hp,
        Concentration,
        SightRange,
        Speed,
        ActionPoint,
        AdditionalHitRate,
        CriticalChance,
        CriticalDamage,
        EquippedWeapon,
        BehaviorPattern,
        EnemySkill,
        RewardGold,
        RewardXP,
        RewardItem,
        RewardWeapon
    }
    #endregion
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
            var curData = new EnemyData();

            curData.index = int.Parse(data[(int)Col.Index]);
            curData.nameIndex = int.Parse(data[(int)Col.EnemyName]);
            curData.stat = new UnitStat();
            curData.weaponIndex = int.Parse(data[(int)Col.EquippedWeapon]);
            curData.btIndex = int.Parse(data[(int)Col.BehaviorPattern]);
            curData.model = Resources.Load("Prefab/Units/" + data[(int)Col.ModelFile]) as GameObject;   

            curData.rewardGold = int.Parse(data[(int)Col.RewardGold]);
            curData.rewardExp = int.Parse(data[(int)Col.RewardXP]);
            curData.rewardItem = data[(int)Col.RewardItem];
            curData.rewardWeapon = data[(int)Col.RewardWeapon];

            curData.stat.SetOriginalStat(StatType.MaxHp, int.Parse(data[(int)Col.Hp]));
            curData.stat.SetOriginalStat(StatType.CurHp, int.Parse(data[(int)Col.Hp]));
            curData.stat.SetOriginalStat(StatType.Concentration, int.Parse(data[(int)Col.Concentration]));
            curData.stat.SetOriginalStat(StatType.SightRange, int.Parse(data[(int)Col.SightRange]));
            curData.stat.SetOriginalStat(StatType.Speed, int.Parse(data[(int)Col.Speed]));
            curData.stat.SetOriginalStat(StatType.MaxActionPoint, int.Parse(data[(int)Col.ActionPoint]));
            curData.stat.SetOriginalStat(StatType.CurActionPoint, 0);
            curData.stat.SetOriginalStat(StatType.AdditionalHitRate, int.Parse(data[(int)Col.AdditionalHitRate]));
            curData.stat.SetOriginalStat(StatType.CriticalChance, int.Parse(data[(int)Col.CriticalChance]));
            curData.stat.SetOriginalStat(StatType.RevolverAdditionalDamage, 0);
            curData.stat.SetOriginalStat(StatType.RevolverAdditionalRange, 0);
            curData.stat.SetOriginalStat(StatType.RevolverCriticalDamage, int.Parse(data[(int)Col.CriticalDamage]));
            curData.stat.SetOriginalStat(StatType.RepeaterAdditionalDamage, 0);
            curData.stat.SetOriginalStat(StatType.RepeaterAdditionalRange, 0);
            curData.stat.SetOriginalStat(StatType.RepeaterCriticalDamage, int.Parse(data[(int)Col.CriticalDamage]));
            curData.stat.SetOriginalStat(StatType.ShotgunAdditionalDamage, 0);
            curData.stat.SetOriginalStat(StatType.ShotgunAdditionalRange, 0);
            curData.stat.SetOriginalStat(StatType.ShotgunCriticalDamage, int.Parse(data[(int)Col.CriticalDamage]));

            enemyInfos.Add(curData);
        }
    }

    public EnemyData GetInfo(int index)
    {
        if (index == 0)
        {
            Debug.LogWarning("Trying to get Null Enemy Data");
        }
        
        foreach (var data in enemyInfos)
        {
            if (data.index == index) return data;
        }

        Debug.LogError("Wrong index");
        throw new Exception();
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
