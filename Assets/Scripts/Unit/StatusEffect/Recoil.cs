public class Recoil : StatusEffect
{
    public Recoil(Unit creator) : base(creator)
    {
    }

    public override StatusEffectType GetStatusEffectType()
    {
        return StatusEffectType.Recoil;
    }

    public override StatusEffect Combine(StatusEffect other)
    {
        return this;
    }

    public override void OnTurnStarted()
    { }

    public override void OnTurnFinished()
    {
        Delete();
    }

    public override int GetDuration()
    {
        return 0;
    }
}