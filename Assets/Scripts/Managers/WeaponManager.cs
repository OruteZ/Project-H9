using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : Generic.Singleton<WeaponManager>
{
    public List<WeaponData> weaponList;

    public static WeaponData GetWeaponData(int index)
    {
        if (index < instance.weaponList.Count) return instance.weaponList[index];
        
        Debug.LogError("there are " + instance.weaponList.Count + " weapons, thers is no " + index + " index");
        return null;

    }

    public static int GetWeaponIndex(Weapon weapon)
    {
        for (int i = 0; i < instance.weaponList.Count; i++)
        {
            if (instance.weaponList[i].weaponName == weapon.weaponName) return i;
        }

        return -1;
    }
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
