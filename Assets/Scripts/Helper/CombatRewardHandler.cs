using System;
using System.Collections.Generic;
using Castle.Core;
using UnityEngine;

public class CombatRewardHandler
{
    private int _gold;
    private readonly List<Pair<int, int>> _rewardCandidates = new List<Pair<int, int>>();
    private readonly List<int> _rewardItems = new List<int>();
    private readonly ItemDatabase _itemDB;
    
    public CombatRewardHandler(ItemDatabase itemDB)
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
        
        _rewardCandidates.Clear();
        for (int i = 0; i < infoRewardItem.Length; i += 2)
        {
            _rewardCandidates.Add(new Pair<int, int>(infoRewardItem[i], infoRewardItem[i + 1]));
        }
    }
    
    public void ApplyReward()
    {
        // get gold to player inventory
        GameManager.instance.playerInventory.AddGold(_gold);
        
        // even index is item index, odd index is percent
        for (int i = 0; i < _rewardCandidates.Count; i++)
        {
            int index = _rewardCandidates[i].First;
            float percent = _rewardCandidates[i].Second * 0.01f; // in excel, percent is 0 ~ 100;
            
            //check percentage is success
            if (percent >= UnityEngine.Random.Range(0, 1))
            {
                _rewardItems.Add(index);
            }
        }
        
        // get item to player inventory
        for (var i = 0; i < _rewardItems.Count; i++)
        {
            var itemIndex = _rewardItems[i];
            bool success = GameManager.instance.playerInventory.TryAddItem(Item.CreateItem(_itemDB.GetItemData(itemIndex)));
            
            //if fail, remove from reward list
            if (!success)
            {
                _rewardItems.RemoveAt(i);
                i--;
            }
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