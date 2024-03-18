using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatRewardHelper
{
    private int _gold;
    private readonly List<int> _rewardItems = new List<int>();
    private readonly ItemDatabase _itemDB;
    
    public CombatRewardHelper(ItemDatabase itemDB)
    {
        _gold = 0;
        _rewardItems.Clear();
        _itemDB = itemDB;
    }

    public void AddGold(int infoRewardGold)
    {
        _gold = infoRewardGold;
    }

    public void AddItem(int[] infoRewardItem)
    {
        if (infoRewardItem.Length == 0) return;
        if (infoRewardItem.Length % 2 == 1)
        {
            Debug.LogError("Reward Item's length is not even");
        }
        
        // even index is item index, odd index is percent
        for (int i = 0; i < infoRewardItem.Length; i += 2)
        {
            int index = infoRewardItem[i];
            float percent = infoRewardItem[i + 1] * 0.01f; // in excel, percent is 0 ~ 100;
            
            //check percentage is success
            if (percent >= UnityEngine.Random.Range(0, 1))
            {
                _rewardItems.Add(index);
            }
        }
    }
    
    public void ApplyReward()
    {
        // get gold to player inventory
        // GameManager.instance.playerInventory.AddGold(_gold);
        
        // get item to player inventory
        foreach (var itemIndex in _rewardItems)
        {
            GameManager.instance.playerInventory.AddItem(Item.CreateItem(_itemDB.GetItemData(itemIndex)));
        }
    }
    
    /// <summary>
    /// 현재 전투 보상 Gold를 반환합니다. 보상 적용은 따로 하니까 읽는데만 사용하도록 합니다.
    /// </summary>
    /// <returns></returns>
    public int GetRewardGold()
    {
        return _gold;
    }
    
    /// <summary>
    /// 전투 보상 아이템들의 Index를 반환합니다. 보상 적용은 따로 하니까 읽는데만 사용하도록 합니다.
    /// </summary>
    /// <returns></returns>
    public int[] GetRewardItemInfos()
    {
        return _rewardItems.ToArray();
    }
}