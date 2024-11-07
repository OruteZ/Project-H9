using UnityEngine;

/// <summary>
/// Use �Լ��� ȣ���� ��, Target ��ġ ���� Unit�� ã�� ItemEffectAmount��ŭ�� �������� �����ϴ�.
/// </summary>
public class DamageItem : Item
{
    public DamageItem(ItemData data) : base(data)
    {
    }

    public override bool Use(Unit user, Vector3Int target)
    {
        var unit = FieldSystem.unitSystem.GetUnit(target);
        if (unit == null) return false;
        
        // build damage context
        int dmg = GetData().itemEffectAmount;
        Damage damageContext = new Damage(dmg, dmg, GetData().damageType, user, null, unit);
        
        unit.TakeDamage(damageContext);
        stackCount--;
        
        GameManager.instance.playerInventory.CollectZeroItem();
        return true;
    }

    public override bool TryEquip()
    {
        return false;
    }
}