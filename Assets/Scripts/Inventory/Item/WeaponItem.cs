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
}