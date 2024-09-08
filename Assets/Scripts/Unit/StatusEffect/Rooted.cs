public class Rooted : StatusEffect
{
    private int _duration;
    
    public Rooted(Unit creator, int duration) : base(creator)
    {
        _duration = duration;
    }

    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.Rooted;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        return this;
    }

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
}