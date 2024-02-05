namespace Inventory.Item
{
    public class WeaponItem : Item
    {
        public WeaponItem(ItemData data) : base(data)
        { }


        public override bool TryEquip()
        {
            GameManager.instance.playerWeaponIndex =
                GetData().id;

            return true;
        }
    }
}