using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldTrap : TileObject
{
    [SerializeField] protected int damage;
    [SerializeField] protected int duration;

    public UnityEvent onTrapTriggered = new UnityEvent();

    public override string[] GetArgs()
    {
        return new string[] { damage.ToString(), duration.ToString() };
    }
    public override void SetArgs(string[] args)
    {
        if (args.Length != 2) throw new Exception("Invalid args length. Expected 2.");

        damage = int.Parse(args[0]);
        duration = int.Parse(args[1]);
    }

    public override void OnHexCollisionEnter(Unit other)
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

        // invoke
        onTrapTriggered.Invoke();

        // remove trap
        RemoveSelf();
    }
}
