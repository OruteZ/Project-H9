namespace PassiveSkill
{
    public class SightRangeUp : BaseEffect
    {
        public SightRangeUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.SightRangeUp;

        public override void Enable()
        {
            unit.GetStat().sightRange += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().sightRange -= (int)amount;
        }

    }
}