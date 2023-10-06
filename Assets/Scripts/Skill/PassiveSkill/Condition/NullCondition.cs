namespace PassiveSkill
{
    public class NullCondition : BaseCondition
    {
        public NullCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType()
        {
            return ConditionType.Null;
        }

        protected override void ConditionSetup()
        {
            FieldSystem.onCombatAwake.AddListener(() => passive.EnableCondition());
        }
    }
}