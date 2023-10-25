namespace PassiveSkill
{
    public class RevengeCondition : BaseCondition
    {
        public RevengeCondition(float amt) : base(amt)
        {
        }

        public override ConditionType GetConditionType()
        {
            return ConditionType.Revenge;
        }

        protected override void ConditionSetup()
        {
            FieldSystem.unitSystem.onAnyUnitDead.AddListener(Check);
        }

        private void Check(Unit deadUnit)
        {
            //if deadUnit is Enemy unit and this unit is Enemy unit, enable passive
            if (deadUnit is Enemy && unit is Enemy)
                passive.EnableCondition();
        }
    }
}