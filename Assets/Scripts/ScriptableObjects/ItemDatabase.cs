using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 1)]
public class ItemDatabase : ScriptableObject
{
    [SerializeField]
    private List<ItemData> _itemList;
    [SerializeField]
    private List<ItemScript> _itemScriptList;

    [ContextMenu("Read Csv")]
    public void ReadCsv()
    {
        var dataList = FileRead.Read("ItemTable", out var columnInfo);

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
            curData.icon = Resources.Load<Sprite>("ItemIcon/" + data[17]);
            curData.itemModel = Resources.Load<GameObject>("Prefab/Item/" + data[18]);   

            if(curData.icon is null) curData.icon = Resources.Load<Sprite>($"ItemIcon/Default");
            
            _itemList.Add(curData);
        }

        ReadScriptTable();
    }
    private void ReadScriptTable()
    {
        var dataList = FileRead.Read("ItemScriptTable", out var columnInfo);

        if (_itemScriptList is null) _itemScriptList = new List<ItemScript>();
        else _itemScriptList.Clear();

        foreach (var data in dataList)
        {
            ItemScript curData = new ItemScript(int.Parse(data[0]), data[1], data[3]);
            _itemScriptList.Add(curData);
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
    public ItemData GetItemData(string name)
    {
        foreach (var item in _itemList)
        {
            if (GetItemScript(item.nameIdx).GetName() == name) return item;
        }

        Debug.LogError("There is no item that has name " + name);
        return null;
    }

    public List<ItemData> GetAllItemData()
    {
        return _itemList;
    }

    public ItemScript GetItemScript(int nameIndex)
    {
        foreach (var script in _itemScriptList)
        {
            if (script.GetIndex() == nameIndex) return script;
        }

        Debug.LogError("There is no script that has id " + nameIndex);
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
    public Sprite icon;
    public GameObject itemModel;

    /// <summary>
    /// 해당 아이템의 툴팁 설명을 구성하여 불러옵니다.
    /// </summary>
    public string GetInventoryTooltipContents()
    {
        string description = "";
        if (itemType == ItemType.Revolver || itemType == ItemType.Repeater || itemType == ItemType.Shotgun)
        {
            string weaponTypeText = itemType.ToString();
            string weaponDamageText = weaponDamage.ToString() + " Damage";
            string weaponRangeText = itemRange.ToString() + " Range";
            string weaponEffect = GameManager.instance.itemDatabase.GetItemScript(nameIdx).GetDescription(this);
            description = weaponTypeText + "\n" + weaponDamageText + "\n" + weaponRangeText + "\n\n" + weaponEffect;
        }
        else
        {
            string typeText = itemType.ToString() + " Item";
            string effect = GameManager.instance.itemDatabase.GetItemScript(nameIdx).GetDescription(this);
            description = typeText + "\n\n" + effect;
        }

        return description;
    }
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