using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class VFXHelper 
{
    public static bool TryGetGunFireFXInfo(ItemType type, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (type)
        {
            case ItemType.Revolver: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
            case ItemType.Repeater: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
            case ItemType.Shotgun: 
                fxKey = "Prefab/VFX/FX_GunSmoke_01";
                time = 2.0f;
                return true;
        }

        return false;
    }

    public static bool TryGetTraceOfBulletFXKey(ItemType type, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (type)
        {
            case ItemType.Revolver: 
            case ItemType.Repeater:
            case ItemType.Shotgun:
                // 총기 종류에 상관없이 같은 탄환 궤적
                fxKey = "Prefab/VFX/BulletLineRender";
                time = 0.1f;
                return true;
            default: return false;
        }

        return false;
    }

    public static bool TryGetBloodingFXKey(ItemType shootedWeaponType, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;

        switch (shootedWeaponType)
        {
            case ItemType.Revolver: 
            case ItemType.Repeater:
            case ItemType.Shotgun:
                // 총기 종류에 상관없이 같은 출혈
                fxKey = "Prefab/VFX/FX_BloodSplat_01";
                time = 2.5f;
                return true;
            default: return false;
        }

        return false;
    }
    
    public static bool TryGetStatusEffectFXKey(StatusEffectType type, out string fxKey, out float time)
    {
        fxKey = string.Empty;
        time = default;
        
        if (type is StatusEffectType.Bleeding or StatusEffectType.Burning or StatusEffectType.Stun)
        {
            fxKey = "Prefab/VFX/FX_" + type.ToString();
            time = -1;
            return true;
        }

        return false;
    }
}
