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
    public WeaponData GetData(int index)
    {
        foreach (var data in weaponList)
        {
            if (data.index == index) return data;
        }
        
        Debug.LogError("there are " + weaponList.Count + " weapons, there is no " + index + " index. returns default weapon");
        return weaponList[0];
    }
    
    public Weapon Clone(int dataIndex)
    {
        var data = GetData(dataIndex);
        Weapon weapon = data.type switch
        {
            ItemType.Character => new Melee(data),
            ItemType.Revolver => new Revolver(data),
            ItemType.Repeater => new Repeater(data),
            ItemType.Shotgun => new Shotgun(data),
            _ => throw new ArgumentOutOfRangeException()
        };
        //
        // weapon.unit = owner;
        // // ReSharper disable once MergeConditionalExpression
        // weapon.unitStat = weapon.unit is null ? new UnitStat() : weapon.unit.GetStat();
        
        //SetUpGimmicks
        
        return weapon;
    }
    
    public static Weapon GetWeapon(int index)
    {
        return GameManager.instance.weaponDatabase.Clone(index);
    }
    
    public static Weapon GetWeapon(WeaponData data)
    {
        return GameManager.instance.weaponDatabase.Clone(data.index);
    }

    [ContextMenu("Load Csv")]
    public void LoadCsv()
    {
        var dataList = FileRead.Read("ItemTable", out var columnInfo);
        
        if (weaponList is null) weaponList = new List<WeaponData>();
        else weaponList.Clear();

        foreach (var d in dataList)
        {
            if((ItemType) Enum.Parse(typeof(ItemType), d[2]) 
               is not (ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun)) continue;
            
            var curData = new WeaponData
            {
                // 0, 1, 2, 4, 10, 11, 12, 13, 14, 16, 17, 18
                
                index = int.Parse(d[0]),
                weaponNameIndex = int.Parse(d[1]),
                type = (ItemType) Enum.Parse(typeof(ItemType), d[2]),
                weaponRange = int.Parse(d[4]),
                weaponDamage = int.Parse(d[10]),
                weaponAmmo = int.Parse(d[11]),
                weaponHitRate = int.Parse(d[12]),
                weaponCriticalChance = int.Parse(d[13]),
                weaponCriticalDamage = int.Parse(d[14]),
                weaponScript = int.Parse(d[15]),
                weaponModel = Resources.Load<GameObject>("Prefab/Item/" + d[18])
            };
            
            //if weaponModel is null, set default model
            if (curData.weaponModel == null)
            {
                curData.weaponModel = curData.type switch
                {
                    ItemType.Revolver => Resources.Load<GameObject>("Prefab/Item/SM_Wep_Revolver_01"),
                    ItemType.Repeater => Resources.Load<GameObject>("Prefab/Item/SM_Wep_Rifle_01"),
                    ItemType.Shotgun => Resources.Load<GameObject>("Prefab/Item/SM_Wep_Shotgun_01"),
                    _ => curData.weaponModel
                };
            }
            
            weaponList.Add(curData);
        }
    }
}


[Serializable]
public class WeaponData
{
    public int index;
    public int weaponNameIndex;
    public ItemType type;
    
    public int weaponRange;
    
    public int weaponDamage;
    public int weaponAmmo;
    public int weaponHitRate;
    public int weaponCriticalChance;
    public int weaponCriticalDamage;
    
    public int weaponScript;
    public GameObject weaponModel;
}
