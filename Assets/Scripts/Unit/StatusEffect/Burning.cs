
using System.Collections.Generic;
using System.Linq;

public class Burning : StatusEffect
{
    private readonly SortedSet<StackInfo> _stackInfos = new (new StackCompare());

    public Burning(int damage, int duration)
    {
        AddStack(damage, duration);
    }

    public Burning()
    { }

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
        controller.GetUnit().TakeDamage(Damage);
    }

    public override void OnTurnFinished()
    {
        int currentTurn = FieldSystem.turnSystem.turnNumber;
        while (_stackInfos.Count > 0)
        {
            var info = _stackInfos.First();
            if (info.finishTurn <= currentTurn)
            {
                Damage -= info.damage;
                _stackInfos.Remove(info);
            }
            else break;
        }

        //if queue is empty, delete this status effect.
        if (_stackInfos.Count == 0)
        {
            Delete();
        }
    }

    public override int GetDuration()
    {
        return _stackInfos.Last().finishTurn - FieldSystem.turnSystem.turnNumber;
    }

    #region PRIVATE
    private void AddStack(int damage, int duration)
    {
        duration += FieldSystem.turnSystem.turnNumber;
        AddStack(new StackInfo{damage = damage, finishTurn = duration});
    }

    private void AddStack(StackInfo info)
    {
        Damage += info.damage;
        
        foreach (var stackInfo in _stackInfos)
        {
            if (stackInfo.finishTurn == info.finishTurn)
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
    public int finishTurn;
}

internal class StackCompare : IComparer<StackInfo>
{
    public int Compare(StackInfo big, StackInfo small)
    {
        return big.finishTurn - small.finishTurn;
    }
}
