using UnityEngine;

public class BuffItem : Item
{
    public BuffItem(ItemData data) : base(data)
    {
    }

    public override bool Use(Unit user, Vector3Int target)
    {
        int buffStatTypeIndex = GetData().itemEffect;
        
        StatType statType = (StatType)buffStatTypeIndex;
        int amount = GetData().itemEffectAmount;
        int duration = GetData().itemEffectDuration;

        var itemBuff = new ItemBuff(GetData().id, statType, amount, duration, user);

        return true;
    }

    public override bool TryEquip()
    {
        return false;
    }
}

public class ItemBuff : IDisplayableEffect
{
    private readonly int _itemIndex;
    private readonly StatType _statType;
    private readonly int _amount;
    private readonly Unit _unit;
    
    private int _duration;

    public bool Add(ItemBuff other)
    {
        //if index is different, return false
        if (_itemIndex != other._itemIndex) return false;
        
        //add duration
        _duration += other._duration;
        return true;
    }

    public ItemBuff(int index, StatType statType, int amount, int duration, Unit unit)
    {
        _itemIndex = index;
        _statType = statType;
        _amount = amount;
        _duration = duration;
        _unit = unit;
        
        //get units buffs
        var buffs = _unit.GetDisplayableEffects();
        foreach (var buff in buffs)
        {
            if (buff is ItemBuff itemBuff)
            {
                if (itemBuff.Add(this))
                {
                    return;
                }
            }
        }
        
        Setup();
    }
    
    private void Setup()
    {
        ApplyBuff();
        _unit.AddDisplayableEffect(this);
        
        FieldSystem.turnSystem.onTurnChanged.AddListener(DecreaseDuration);
        FieldSystem.onCombatFinish.AddListener(RemoveBuff);
    }

    public void ApplyBuff()
    {
        _unit.stat.Add(_statType, _amount);
    }

    public void RemoveBuff(bool none = false)
    {
        _unit.stat.Subtract(_statType, _amount);
        
        FieldSystem.turnSystem.onTurnChanged.RemoveListener(DecreaseDuration);
        FieldSystem.onCombatFinish.RemoveListener(RemoveBuff);
        _unit.RemoveDisplayableEffect(this);
    }

    public void DecreaseDuration()
    {
        _duration--;
        if (_duration <= 0)
        {
            RemoveBuff();
        }
    }
    
    #region IDisplayableEffect
    public int GetIndex()
    {
        return _itemIndex;
    }

    public int GetStack()
    {
        return _amount;
    }

    public int GetDuration()
    {
        return _duration;
    }

    public bool CanDisplay()
    {
        return true;
    }
    #endregion
}