namespace PassiveSkill
{
    public class DyingCondition : BaseCondition
    {
        public DyingCondition(float amt) : base(amt)
        {
        }

        public override ConditionType GetConditionType() => ConditionType.Dying;

        protected override void ConditionSetup()
        {
            unit.onDead.AddListener((_) => passive.FullfillCondition(this));
        }
    }
}