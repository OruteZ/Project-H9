using UnityEngine;

public class CleanseItem : Item
{
    public CleanseItem(ItemData data) : base(data)
    {
    }

    public override bool Use(Unit user, Vector3Int target)
    {
        int debuffStatTypeIndex = GetData().itemEffect;

        StatusEffectType statType = (StatusEffectType)debuffStatTypeIndex;

        var statusEffect = user.GetDisplayableEffects();
        foreach (var debuff in statusEffect)
        {
            if (debuff is StatusEffect itemDebuff && itemDebuff.GetStatusEffectType() == statType)
            { 
                //remove this status effect
                user.TryRemoveStatus(itemDebuff);
                stackCount--;
                
                GameManager.instance.playerInventory.CollectZeroItem();
                return true;
            }
        }

        return false;
    }

    public override bool TryEquip()
    {
        return false;
    }
}