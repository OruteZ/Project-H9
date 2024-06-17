namespace PassiveSkill
{
    public class NullCondition : BaseCondition
    {
        public NullCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.Null;

        protected override void ConditionSetup()
        {
            FieldSystem.onStageAwake.AddListener(EnablePassive);
        }

        public override void OnDelete()
        {
            base.OnDelete();
            FieldSystem.onStageAwake.RemoveListener(EnablePassive);
        }
        
        private void EnablePassive()
        {
            passive.FullfillCondition(this);
        }
    }
}