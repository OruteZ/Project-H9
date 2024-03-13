using UnityEngine;

public class EtcItem : Item
{
    public EtcItem(ItemData data) : base(data)
    {
    }
    public override bool Use(Unit user, Vector3Int target)
    {
        return false;
    }

    public override bool TryEquip()
    {
        return false;
    }
}