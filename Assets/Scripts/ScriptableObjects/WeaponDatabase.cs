using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "ScriptableObjects/WeaponDB", order = 1)]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> weaponList;
    public List<string> weaponNameTable;
    public List<string> weaponScriptTable;

    public int indexStart = 101;

    public WeaponData GetWeaponData(int index)
    {
        index -= indexStart;
        if (index < weaponList.Count && 0 <= index) return weaponList[index];
        
        Debug.LogError("there are " + weaponList.Count + " weapons, there is no " + index + " index");
        return null;
    }
    
    public Weapon Clone(int dataIndex)
    {
        var data = GetWeaponData(dataIndex);
        Weapon weapon = data.type switch
        {
            WeaponType.Null => null,
            WeaponType.Character => new Character(),
            WeaponType.Revolver => new Revolver(),
            WeaponType.Repeater => new Repeater(),
            WeaponType.Shotgun => new Shotgun(),
            _ => throw new System.ArgumentOutOfRangeException()
        };
        //
        // weapon.unit = owner;
        // // ReSharper disable once MergeConditionalExpression
        // weapon.unitStat = weapon.unit is null ? new UnitStat() : weapon.unit.GetStat();

        weapon.nameIndex = data.weaponNameIndex;
        weapon.model = data.weaponModel;
        weapon.weaponDamage = data.weaponDamage;
        weapon.weaponRange = data.weaponRange;
        weapon.maxEmmo = data.weaponAmmo;
        weapon.currentEmmo = data.weaponAmmo;
        weapon.hitRate = data.weaponHitRate;
        weapon.criticalChance = data.weaponCriticalChance;
        weapon.criticalDamage = data.weaponCriticalDamage;
        weapon.script = data.weaponScript;
        
        //SetUpGimmicks
        
        return weapon;
    }

    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("WeaponTable");
        
        weaponList?.Clear();

        for (var i = 0; i < dataList.Count; i++)
        {
            var curData = new WeaponData
            {
                weaponNameIndex = int.Parse(dataList[i][1]),
                type = (WeaponType)int.Parse(dataList[i][2]),
                weaponRange = int.Parse(dataList[i][3]),
                weaponDamage = int.Parse(dataList[i][4]),
                weaponAmmo = int.Parse(dataList[i][5]),
                weaponHitRate = int.Parse(dataList[i][6]),
                weaponCriticalChance = int.Parse(dataList[i][7]),
                weaponCriticalDamage = int.Parse(dataList[i][8]),
                //weaponPrice;
                //weaponSkill;
                weaponScript = int.Parse(dataList[i][11])
            };
            
            weaponList.Add(curData);
        }
    }
}


[Serializable]
public class WeaponData
{
    public int weaponNameIndex;
    public GameObject weaponModel;
    public WeaponType type;
    public int weaponDamage;
    public int weaponRange;
    public int weaponAmmo;
    public float weaponHitRate;
    public float weaponCriticalChance;
    public float weaponCriticalDamage;

    public int weaponScript;
}
