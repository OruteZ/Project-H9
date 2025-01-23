using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pitfall : FieldTrap
{
    public override void OnHexCollisionEnter(Unit other)
    {
        StatusEffect stun = new Stun(duration, null);
        if (!other.TryAddStatus(stun))
        {
            Debug.LogError("상태이상이 적용되지 않음");
        }

        // invoke
        onTrapTriggered.Invoke();

        // remove trap
        RemoveSelf();
    }
}
