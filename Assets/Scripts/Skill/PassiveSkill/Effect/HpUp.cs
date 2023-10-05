namespace PassiveSkill
{
    public class HpUp : BaseEffect
    {
        public HpUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.HpUp;

        public override void Enable()
        {
            unit.GetStat().maxHp += (int)amount;
            unit.GetStat().curHp += (int)amount;
        }

        public override void Disable()
        {
            var stat = unit.GetStat();
            
            stat.maxHp -= (int)amount;

            if (stat.curHp > stat.maxHp)
            {
                stat.curHp = stat.maxHp;
            }
        }
    }
}