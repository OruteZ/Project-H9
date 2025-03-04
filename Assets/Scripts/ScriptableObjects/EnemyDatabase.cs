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
        Level,
    }
    #endregion
    [SerializeField] 
    private List<EnemyData> enemyInfos;

    private List<EnemyName> _enemyNames;
    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("EnemyTable", out var columnInfo);

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
            curData.modelName = data[(int)Col.ModelFile];
            curData.model = Resources.Load("Prefab/UnitModels/" + data[(int)Col.ModelFile]) as GameObject;  
            curData.skillIndex = FileRead.ConvertStringToArray<int>(data[(int)Col.EnemySkill]);

            curData.rewardGold = int.Parse(data[(int)Col.RewardGold]);
            curData.rewardExp = int.Parse(data[(int)Col.RewardXP]);
            curData.rewardItem = FileRead.ConvertStringToArray<int>(data[(int)Col.RewardItem]);

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

            curData.level = int.Parse(data[(int)Col.Level]);

            enemyInfos.Add(curData);
        }
    }

    private void LoadEnemyNameScript()
    {
        var dataList = FileRead.Read("EnemyLocalizationTable", out var columnInfo);

        if (_enemyNames is null) _enemyNames = new List<EnemyName>();
        else _enemyNames.Clear();

        foreach (var data in dataList)
        {
            var curData = new EnemyName();

            curData.nameIndex = int.Parse(data[0]);
            curData.name = data[(int)UIManager.instance.scriptLanguage];
            curData.Ename = data[2];

            _enemyNames.Add(curData);
        }
    }
    public string GetEnemyName(int nameIndex)
    {
        if (_enemyNames is null) LoadEnemyNameScript();

        foreach (var nameInfo in _enemyNames)
        {
            if (nameInfo.nameIndex == nameIndex) 
            {
                return nameInfo.name;
            }
        }
        Debug.Log("Can't Find Enemy Name. nameIndex : " + nameIndex);
        return "";
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

        Debug.LogError("Wrong index: " + index);
        throw new Exception();
    }
}

[System.Serializable]
public struct EnemyData
{
    public int index;
    public int nameIndex;
    public string modelName;
    public GameObject model;
    
    public EnemyType enemyType;
    
    [Header("UnitStat")]
    public UnitStat stat;
    
    public int weaponIndex;
    public int btIndex;
    
    public int[] skillIndex;
    
    public int rewardGold;
    public int rewardExp;
    public int[] rewardItem;

    public int level;
}

public struct UnitModelData
{
    public string name;
    public GameObject modelObject;
}
public struct EnemyName
{
    public int nameIndex;
    public string Ename;
    public string name;
}

public enum EnemyType
{
    NORMAL = 0 ,
    BOSS_LINSDALE = 1,
    BOSS_HEINRICH = 2,
    BOSS_JACKSON = 3,
}
