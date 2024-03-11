using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 1)]
public class ItemDatabase : ScriptableObject
{
    [SerializeField]
    private List<ItemData> _itemList;

    [ContextMenu("Read Csv")]
    public void ReadCsv()
    {
        var dataList = FileRead.Read("ItemTable");

        if (_itemList is null) _itemList = new List<ItemData>();
        else _itemList.Clear();

        foreach(var data in dataList)
        {
            ItemData curData = new ItemData();
            
            curData.id = int.Parse(data[0]);
            curData.nameIdx = int.Parse(data[1]);
            curData.itemType = (ItemType)Enum.Parse(typeof(ItemType), data[2]);
            curData.itemMaxStorage = int.Parse(data[3]);
            curData.itemRange = int.Parse(data[4]);
            curData.sweetSpot = int.Parse(data[5]);
            curData.itemEffect = int.Parse(data[6]);
            curData.itemEffectAmount = int.Parse(data[7]);
            curData.itemEffectDuration = int.Parse(data[8]);
            curData.itemPrice = int.Parse(data[9]);
            curData.weaponDamage = int.Parse(data[10]);
            curData.weaponAmmo = int.Parse(data[11]);
            curData.weaponHitRate = int.Parse(data[12]);
            curData.weaponCriticalChance = int.Parse(data[13]);
            curData.weaponCriticalDamage = int.Parse(data[14]);
            curData.weaponSkill = int.Parse(data[15]);
            curData.itemTooltip = int.Parse(data[16]);
            // curData.icon = Resources.Load<Texture2D>("ItemIcon/" + data[17]);
            curData.itemModel = data[18];

            _itemList.Add(curData);
        }
    }

    public ItemData GetItemData(int id)
    {
        foreach (var item in _itemList)
        {
            if (item.id == id) return item;
        }

        Debug.LogError("There is no item that has id " + id);
        return null;
    }
}

[Serializable]
public class ItemData
{
    public int id;
    public int nameIdx;
    public ItemType itemType;
    public int itemMaxStorage;
    public int itemRange;
    public int sweetSpot;
    public int itemEffect;
    public int itemEffectAmount;
    public int itemEffectDuration;
    public int itemPrice;
    public int weaponDamage;
    public int weaponAmmo;
    public int weaponHitRate;
    public int weaponCriticalChance;
    public int weaponCriticalDamage;
    public int weaponSkill;
    public int itemTooltip;
    public Texture2D icon;
    public string itemModel;
}

public enum ItemType
{
    Null,
    Etc,
    Character,
    Revolver,
    Repeater,
    Shotgun,
    Heal,
    Damage,
    Cleanse,
    Buff,
    Debuff,
}