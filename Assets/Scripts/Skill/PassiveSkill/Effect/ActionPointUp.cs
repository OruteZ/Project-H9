namespace PassiveSkill
{
    public class ActionPointUp: BaseEffect
    {
        public ActionPointUp(float amount) : base(amount)
        { }
        
        protected override void EffectSetup()
        { }

        public override PassiveEffectType GetEffectType() => PassiveEffectType.ActionPointUp;

        public override void Enable()
        {
            unit.GetStat().actionPoint += (int)amount;
        }

        public override void Disable()
        {
            unit.GetStat().actionPoint -= (int)amount;
        }

    }
}