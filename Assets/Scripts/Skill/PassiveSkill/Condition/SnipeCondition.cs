namespace PassiveSkill
{
    public class SnipeCondition : BaseCondition
    {
        public SnipeCondition(float amt) : base(amt)
        {
        }

        public override ConditionType GetConditionType() => ConditionType.Snipe;

        protected override void ConditionSetup()
        {
            //on any unit moved. check condition
            FieldSystem.unitSystem.onAnyUnitMoved.AddListener(Check);
        }

        private void Check(Unit nullUnit)
        {
            //If there is no enemy within 'amount' tiles of the unit, activate the passive
            var positions = Hex.GetCircleGridList((int)amount, unit.hexPosition);
            foreach (var pos in positions)
            {
                var target = FieldSystem.unitSystem.GetUnit(pos);
                if (target is Enemy)
                {
                    passive.NotFullfillCondition(this);
                    return;   
                }
            }
            
            passive.FullfillCondition(this);
        }
    }
}