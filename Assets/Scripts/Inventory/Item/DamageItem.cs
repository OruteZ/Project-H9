using UnityEngine;

/// <summary>
/// Use 함수를 호출할 때, Target 위치 내의 Unit을 찾아 ItemEffectAmount만큼의 데미지를 입힙니다.
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
        
        unit.TakeDamage(GetData().itemEffectAmount, user);
        stackCount--;
        return true;
    }

    public override bool TryEquip()
    {
        return false;
    }
}