using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataBase : Generic.Singleton<WeaponDataBase>
{
    public List<WeaponData> weaponList;
}

[Serializable]
public class WeaponData
{
    public string weaponName;
    public GameObject weaponModel;
    public WeaponType type;
    public int baseDamage;
    public int baseRange;
    public List<GimmickType> gimmicks;
}
