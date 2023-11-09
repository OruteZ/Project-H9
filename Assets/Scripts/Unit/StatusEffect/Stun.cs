public class Stun : StatusEffect
{
    private int _duration;
    
    public Stun(int duration)
    {
        _duration = duration;
    }
    public override StatusEffectType GetStatusEffectType() => StatusEffectType.Stun;
    public override void OnTurnStarted()
    { }

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