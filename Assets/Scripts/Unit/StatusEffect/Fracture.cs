public class Fracture : StatusEffect
{
    //duration system like stun
    private int _duration;
    
    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.Fracture;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        //combine duration
        _duration += other.GetDuration();
        return this;
    }

    public override void OnTurnStarted()
    { }

    public override void OnTurnFinished()
    {
        //calcuate duration
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