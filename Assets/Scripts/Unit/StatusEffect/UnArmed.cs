﻿public class UnArmed : StatusEffect
{
    private int _duration;
    
    //constructor that initialize duration
    public UnArmed(int duration, Unit creator) : base(creator)
    {
        _duration = duration;
    }
    
    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.UnArmed;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        _duration += other.GetDuration();
        return this;
    }

    public override void OnTurnStarted()
    { }

    public override void OnTurnFinished()
    {
        _duration -= 1;
        if(_duration <= 0) Delete();
    }

    public override int GetDuration()
    {
        return _duration; 
    }
}