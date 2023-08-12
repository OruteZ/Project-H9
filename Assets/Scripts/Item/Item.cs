using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//개선점: 아이템을 상속하는 weaponItem, usableItem, otherItem으로 분할하는 것이 좋아보임.

/// <summary>
/// Item의 속성을 저장하고 초기화하는 클래스
/// </summary>
public class ItemInfo
{
    public enum ItemCategory
    {
        Null,
        Weapon,
        Usable,
        Other
    }
    public int index { get; private set; }
    public int iconNumber { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public ItemCategory category { get; private set; }
    public int price { get; private set; }

    /// <summary>
    /// ItemTable에서 한 행을 입력받아서 변수들을 초기화합니다.
    /// </summary>
    /// <param name="list"> ItemTable에서 가져온 한 행의 문자열 </param>
    public ItemInfo(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(""))
            {
                list[i] = "0";
            }
        }

        index = int.Parse(list[0]);
        iconNumber = int.Parse(list[1]);
        name = list[2];
        description = list[3];
        category = (ItemCategory)int.Parse(list[4]);
        price = 5;//test
    }
}

/// <summary>
/// 아이템의 속성과 여러 상태 및 기능을 포함하는 클래스
/// </summary>
public class Item
{
    public ItemInfo itemInfo { get; private set; }

    public Item(ItemInfo itemInfo)
    {
        this.itemInfo = itemInfo;
    }
}
