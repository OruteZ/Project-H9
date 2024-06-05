using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemScript : IItemScript
{
    [SerializeField] private int _itemIndex;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemDescription;
    private static string[] _itemColumm = 
        {   
            "itemMaxStorage",
            "itemRange",
            "sweetSpot",
            "itemEffect", 
            "itemEffectAmount", 
            "itemEffectDuration", 
            "itemPrice",
            "weaponDamage", 
            "weaponAmmo",
            "weaponHitRate",
            "weaponCriticalChance", 
            "weaponCriticalDamage" 
        };
    public ItemScript(int idx, string name, string description) 
    {
        _itemIndex = idx;
        _itemName = name;
        _itemDescription = description;
    }

    public int GetIndex()
    {
        return _itemIndex;
    }
    public string GetName()
    {
        return _itemName;
    }
    public string GetDescription()
    {
        ItemData data = GameManager.instance.itemDatabase.GetItemData(_itemIndex);
        if (data == null) return "Null Reference error in ItemScript";

        string description = _itemDescription;
        description = SubstituteKeyword(description);

        description = SubstituteValue(description, _itemColumm[0], data.itemMaxStorage);
        description = SubstituteValue(description, _itemColumm[1], data.itemRange);
        description = SubstituteValue(description, _itemColumm[2], data.sweetSpot);
        description = SubstituteValue(description, _itemColumm[3], data.itemEffect);
        description = SubstituteValue(description, _itemColumm[4], data.itemEffectAmount);
        description = SubstituteValue(description, _itemColumm[5], data.itemEffectDuration);
        description = SubstituteValue(description, _itemColumm[6], data.itemPrice);
        description = SubstituteValue(description, _itemColumm[7], data.weaponDamage);
        description = SubstituteValue(description, _itemColumm[8], data.weaponAmmo);
        description = SubstituteValue(description, _itemColumm[9], data.weaponHitRate);
        description = SubstituteValue(description, _itemColumm[10], data.weaponCriticalDamage);
        description = SubstituteValue(description, _itemColumm[11], data.weaponCriticalChance);
        return description;
    }
    private string SubstituteKeyword(string str)
    {
        string origin = str;
        string[] split = { "<keyword:", ">" };
        string result = "";

        while (origin.Contains(split[0]))
        {
            int startIndex = origin.IndexOf(split[0]);
            int endIndex = startIndex + GetSubString(origin, origin.IndexOf(split[0]), origin.Length).IndexOf(split[1]);
            string beforeString = GetSubString(origin, 0, startIndex);
            string middleString = GetSubString(origin, startIndex + split[0].Length, endIndex);
            string afterString = GetSubString(origin, endIndex + split[1].Length, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);

            string keyword = SkillManager.instance.GetSkillKeyword(int.Parse(middleString)).name;
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, keyword);
            origin = afterString;
        }
        return result + origin;
    }
    private string SubstituteValue(string str, string valueName, float value)
    {
        string origin = str;
        string split = "<" + valueName + ">";
        string result = "";
        while (origin.Contains(split))
        {
            int startIndex = origin.IndexOf(split);
            int endIndex = startIndex + split.Length;
            string beforeString = GetSubString(origin, 0, startIndex);
            string afterString = GetSubString(origin, endIndex, origin.Length);
            result += beforeString;
            string highlightColor = UICustomColor.GetColorHexCode(UICustomColor.PlayerStatColor);

            string valeuStr = value.ToString();
            result += string.Format("<color=#{0}>{1}</color>", highlightColor, valeuStr);
            origin = afterString;
        }
        //Debug.Log(result + origin);
        return result + origin;
    }
    private string GetSubString(string origin, int startIndex, int endIndex)
    {
        int length = endIndex - startIndex;
        return origin.Substring(startIndex, length);
    }
}
