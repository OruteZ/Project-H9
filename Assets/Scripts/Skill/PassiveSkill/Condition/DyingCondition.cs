namespace PassiveSkill
{
    public class DyingCondition : BaseCondition
    {
        public DyingCondition(float amt) : base(amt)
        {
        }

        public override ConditionType GetConditionType()
        {
            return ConditionType.Dying;
        }

        protected override void ConditionSetup()
        {
            unit.onDead.AddListener((_) => passive.EnableCondition());
        }
    }
}