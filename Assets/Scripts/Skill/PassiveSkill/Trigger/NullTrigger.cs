namespace PassiveSkill
{
    public class NullTrigger : BaseTrigger
    {
        public NullTrigger(float amt) : base(amt)
        { }

        public override TriggerType GetTriggerType()
        {
            return TriggerType.Null;
        }

        protected override void TriggerSetup()
        {
            FieldSystem.onCombatAwake.AddListener(() => passive.TurnOnPassive());
        }
    }
}