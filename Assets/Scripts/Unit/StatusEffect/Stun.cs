public class Stun : StatusEffect
{
    private int _duration;
    
    public Stun(int duration, Unit creator) : base(creator)
    {
        _duration = duration;
    }
    public override StatusEffectType GetStatusEffectType() => StatusEffectType.Stun;

    public override void OnTurnStarted()
    {
        // get unit and set it's action points to 0
        controller.GetUnit().stat.SetOriginalStat(StatType.CurActionPoint, 0);
    }

    public override void OnTurnFinished()
    {
        _duration -= 1;
        if (_duration <= 0)
        {
            Delete();
        }
    }

    public override int GetDuration()
    {
        return _duration;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        _duration += other.GetDuration();
        return this;
    }
}