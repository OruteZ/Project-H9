public class WeaponItem : Item
{
    public WeaponItem(ItemData data) : base(data)
    { }


    public override bool TryEquip()
    {
        GameManager.instance.playerWeaponIndex =
            GetData().id;
        UIManager.instance.onWeaponChanged.Invoke();
        return true;
    }
}