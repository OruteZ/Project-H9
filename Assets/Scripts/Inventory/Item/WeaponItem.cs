public class WeaponItem : Item
{
    public WeaponItem(ItemData data) : base(data)   //for ui test
    {
    }


    public override bool TryEquip()
    {
        GameManager.instance.playerWeaponIndex =
            GetData().id;
        UIManager.instance.onWeaponChanged.Invoke();
        return true;
    }
}