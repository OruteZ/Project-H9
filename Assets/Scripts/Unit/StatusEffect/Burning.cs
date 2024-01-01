
using System.Collections.Generic;
using System.Linq;

public class Burning : StatusEffect
{
    private readonly SortedSet<StackInfo> _stackInfos = new (new StackCompare());

    public Burning(int damage, int duration, Unit creator) : base(creator)
    {
        AddStack(damage, duration);
    }

    public override void Setup(UnitStatusEffectController controller)
    {
        base.Setup(controller);
        onStackChanged.AddListener(() =>
        {
            //if stack is 0, delete this status effect
            if (damage <= 0)
            {
                Delete();
            }
        });
    }

    public override StatusEffectType GetStatusEffectType() => StatusEffectType.Burning;

    public override StatusEffect Combine(StatusEffect other)
    {
        foreach (var info in ((Burning)other)._stackInfos)
        {
            AddStack(info);
        }
        return this;
    }

    public override void OnTurnStarted()
    {
        //unit takes damage by stack * 1;
        controller.GetUnit().TakeDamage(damage, creator);
    }

    public override void OnTurnFinished()
    {
        foreach (var stackInfo in _stackInfos)
        {
            stackInfo.duration -= 1;
        }
        
        //find stackInfo that duration is 0, and remove it
        _stackInfos.RemoveWhere((info) => info.duration <= 0);

        //if queue is empty, delete this status effect.
        if (_stackInfos.Count == 0)
        {
            Delete();
        }
    }

    public override int GetDuration()
    {
        return _stackInfos.Last().duration;
    }

    #region PRIVATE
    private void AddStack(int damage, int duration)
    {
        AddStack(new StackInfo{damage = damage, duration = duration});
    }

    private void AddStack(StackInfo info)
    {
        damage += info.damage;
        
        foreach (var stackInfo in _stackInfos)
        {
            if (stackInfo.duration == info.duration)
            {
                stackInfo.damage += info.damage;
                return;
            }
        }
        
        //if cant find same finish turn, add new stack info
        _stackInfos.Add(info);
    }
    #endregion
}

internal class StackInfo
{
    public int damage;
    public int duration;
}

internal class StackCompare : IComparer<StackInfo>
{
    //sort by duration, biggest duration is last at sortedset
    public int Compare(StackInfo x, StackInfo y)
    {
        return x.duration.CompareTo(y.duration);
    }
}
