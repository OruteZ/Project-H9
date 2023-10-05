namespace PassiveSkill
{
    public class ConcentrationUp : BaseEffect
    {
        public ConcentrationUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.ConcentrationUp;

        public override void Enable()
        {
            unit.GetStat().concentration += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().concentration -= (int)amount;
        }

    }
}