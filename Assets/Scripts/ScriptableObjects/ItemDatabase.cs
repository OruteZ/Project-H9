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
            curData.id = int.Parse(data[(int)ItemColumn.Id]);
            curData.nameIdx = int.Parse(data[(int)ItemColumn.NameIdx]);
            curData.stackAble = bool.Parse(data[(int)ItemColumn.StackAble]);
            curData.equipAble = bool.Parse(data[(int)ItemColumn.EquipAble]);
            curData.usable = bool.Parse(data[(int)ItemColumn.Usable]);
            curData.icon = Resources.Load<Texture2D>(data[(int)ItemColumn.IconPath]);
            curData.descriptionIdx = int.Parse(data[(int)ItemColumn.DescriptionIdx]);

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
    public ItemType type;
    
    public int id;
    public int nameIdx;

    public bool stackAble;
    public bool equipAble;
    public bool usable;
    
    public Texture2D icon;
    public int descriptionIdx;
}

public enum ItemColumn
{
    Id,
    NameIdx,
    StackAble,
    EquipAble,
    Usable,
    IconPath,
    DescriptionIdx,
}

public enum ItemType
{
    Weapon,
    Consumable,
}