namespace PassiveSkill
{
    public class CriticalChanceUp : BaseEffect
    {
        public CriticalChanceUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.CriticalChanceUp;

        public override void Enable()
        {
            unit.GetStat().criticalChance += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().criticalChance -= (int)amount;
        }

    }
}