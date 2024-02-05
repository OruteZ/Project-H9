public class WeaponItem : Item
{
    public WeaponItem(ItemData data)
    { }


    public override bool TryEquip()
    {
        GameManager.instance.playerWeaponIndex =
            GetData().id;

        return true;
    }
}