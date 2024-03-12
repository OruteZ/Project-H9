using UnityEngine;

public class WeaponItem : Item
{
    public WeaponItem(ItemData data) : base(data)
    { }


    public override bool Use(Unit user, Vector3Int target)
    {
        return false;
    }

    public override bool TryEquip()
    {
        GameManager.instance.playerWeaponIndex =
            GetData().id;
        UIManager.instance.onWeaponChanged.Invoke();
        return true;
    }

    public override string GetInventoryTooltipContents()
    {
        ItemData data = GetData();
        string weaponTypeText = data.itemType.ToString();
        string weaponDamageText = data.weaponDamage.ToString() + " Damage";
        string weaponRangeText = data.itemRange.ToString() + " Range";
        string weaponEffect = "Effect: " + data.nameIdx.ToString();
        string description = weaponTypeText + "\n" + weaponDamageText + "\n" + weaponRangeText + "\n\n" + weaponEffect;

        return description;
    }
}