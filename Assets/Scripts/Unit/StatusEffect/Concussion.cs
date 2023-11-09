public class Concussion : StatusEffect
{
    //it has duration, and const value to reduce actionPoint;
    private int _duration;
    private const int ACTION_POINT_REDUCE = 1;
    
    //checking effected bool value
    private bool _isEffected = false;
    
    public Concussion(int duration)
    {
        _duration = duration;
    }
    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.Concussion;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        //combine duration
        _duration += other.GetDuration();
        return this;
    }

    public override void OnTurnStarted()
    {
        //if not effected yet, reduce actionPoint
        if (!_isEffected)
        {
            _isEffected = true;
            controller.GetUnit().stat.Subtract(StatType.MaxActionPoint, ACTION_POINT_REDUCE);
        }
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

    //when delete, restore maxActionPoint
    public override void Delete()
    {
        controller.GetUnit().stat.Add(StatType.MaxActionPoint, ACTION_POINT_REDUCE);
        base.Delete();
    }
}