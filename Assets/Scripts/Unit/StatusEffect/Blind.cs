public class Blind : StatusEffect
{
    //copy from Concussion.cs
    private int _duration;
    
    //checking effected bool value
    private bool _isEffected = false;
    
    public Blind(int duration, Unit creator) : base(creator)
    {
        _duration = duration;
    }
    
    //copy from Concussion.cs
    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.Blind;
    }
    
    //copy from Concussion.cs
    public override StatusEffect Combine(StatusEffect other)
    {
        //combine duration
        _duration += other.GetDuration();
        return this;
    }

    public override void Setup(UnitStatusEffectController controller)
    {
        base.Setup(controller);
        
        if (_isEffected is false)
        {
            _isEffected = true;
            controller.GetUnit().stat.SubtractMultiplier(StatType.SightRange, 50);
        }
    }

    //copy from Concussion.cs
    public override void OnTurnStarted()
    {
        //do nothing
    }
    
    //copy from Concussion.cs
    public override void OnTurnFinished()
    {
        _duration -= 1;
        if (_duration <= 0)
        {
            Delete();
        }
    }
    
    //copy from Concussion.cs
    public override int GetDuration()
    {
        return _duration;
    }
    
    //copy from Concussion.cs
    public override void Delete()
    {
        controller.GetUnit().stat.AddMultiplier(StatType.SightRange, 50);
        base.Delete();
    }
}