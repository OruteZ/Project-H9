using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VFXHelper 
{
    public enum FXPattern { None, Fire, TraceOfBullet };

    public static bool TryGetWeaponFXKey(FXPattern pattern, WeaponType type, out string fxKey, out float time)
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
}
