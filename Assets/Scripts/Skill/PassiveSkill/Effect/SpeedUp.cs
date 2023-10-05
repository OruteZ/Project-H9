namespace PassiveSkill
{
    public class SpeedUp : BaseEffect
    {
        public SpeedUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.SpeedUp;

        public override void Enable()
        {
            unit.GetStat().speed += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().speed -= (int)amount;
        }

    }
}