using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VFXHelper 
{
    public static bool TryGetGunFireFXInfo(WeaponType type, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (type)
        {
            case WeaponType.Revolver: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
            case WeaponType.Repeater: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
            case WeaponType.Shotgun: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
        }

        return false;
    }

    public static bool TryGetTraceOfBulletFXKey(WeaponType type, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (type)
        {
            case WeaponType.Null: return false;
            default:
                // 총기 종류에 상관없이 같은 탄환 궤적
                fxKey = "Prefab/VFX/BulletLineRender";
                time = 0.1f;
                return true;
        }

        return false;
    }

    public static bool TryGetBloodingFXKey(WeaponType shootedWeaponType, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (shootedWeaponType)
        {
            case WeaponType.Null: return false;
            default:
                // 총기 종류에 상관없이 같은 출혈
                fxKey = "Prefab/VFX/FX_BloodSplat_01";
                time = 2.5f;
                return true;
        }

        return false;
    }
}
