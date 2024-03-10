using UnityEngine;

public class StatusEffectItem : Item
{
    private StatusEffectType statusEffectType;
    private int duration;

    public StatusEffectItem(ItemData data, StatusEffectType statusEffectType, int duration) : base(data)
    {
        this.statusEffectType = statusEffectType;
        this.duration = duration;
    }

    public override bool Use(Unit user, Vector3Int target)
    {
        // Create an instance of the status effect
        // StatusEffect statusEffect = (StatusEffect)Activator.CreateInstance(statusEffectType, duration, user);

        // Add the status effect to the user
        // user.GetStatusEffectController().AddStatusEffect(statusEffect);

        return true;
    }

    public override bool TryEquip()
    {
        return false;
    }
}