namespace PassiveSkill
{
    public class AdditionalHitRateUp : BaseEffect
    {
        public AdditionalHitRateUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.AdditionalHitRateUp;

        public override void Enable()
        {
            unit.GetStat().additionalHitRate += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().additionalHitRate -= (int)amount;
        }

    }
}