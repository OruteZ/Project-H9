using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Gimmick
{

    public string gimmickName;

    public void Setup(Weapon weapon)
    {
        weapon.onSuccessAttack.AddListener(OnSuccessAttack);
        weapon.onCriticalAttack.AddListener(OnCriticalAttack);
    }

    protected virtual void OnSuccessAttack(Unit target, int damage)
    {
    }

    protected virtual void OnCriticalAttack(Unit target, int damage)
    {
    }

    public static Gimmick Clone(GimmickType gimmickType)
    {
        return gimmickType switch
        {
            GimmickType.CriticalDoubleTab => new CriticalDoubleTab(),
            _ => throw new ArgumentOutOfRangeException(nameof(gimmickType), gimmickType, 
                $"There is no Gimmick that {gimmickType}")
        };
    }
}

public enum GimmickType
{
    CriticalDoubleTab
}
