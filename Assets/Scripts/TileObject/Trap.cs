using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trap : TileObject
{
    [SerializeField] private int damage;
    [SerializeField] private int boundDuration;

    public UnityEvent onTrapTriggered = new UnityEvent();

    public override string[] GetArgs()
    {
        return new string[] { damage.ToString(), boundDuration.ToString() };
    }
    public override void SetArgs(string[] args)
    {
        if (args.Length != 2) throw new Exception("Invalid args length. Expected 2.");

        damage = int.Parse(args[0]);
        boundDuration = int.Parse(args[1]);
    }

    public override void OnCollision(Unit other)
    {
        Damage damageContext = new Damage(
            damage,
            damage,
            Damage.Type.DEFAULT,
            null,
            this,
            other);

        // apply damage
        other.TakeDamage(damageContext);

        // create status effect : bound
        StatusEffect stun = new Stun(boundDuration, null);

        // apply status effect
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
