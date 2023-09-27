namespace PassiveSkill
{
    public class RevolverDamageUp : BaseEffect
    {
        public RevolverDamageUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.RevolverAdditionalDamageUp;

        public override void Enable()
        {
            unit.GetStat().revolverAdditionalDamage += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().revolverAdditionalDamage -= (int)amount;
        }

    }
}