namespace PassiveSkill
{
    public class FighterCondition : BaseCondition
    {
        public FighterCondition(float amt) : base(amt)
        {
        }

        public override ConditionType GetConditionType()
        {
            return ConditionType.Fighter;
        }

        protected override void ConditionSetup()
        {
            //on Any Unit Moved Check
            FieldSystem.unitSystem.onAnyUnitMoved.AddListener(Check);
        }

        private void Check(Unit nullUnit)
        {
            //get all tiles in amount
            var positions = Hex.GetCircleGridList((int)amount, unit.hexPosition);
            foreach (var pos in positions)
            {
                var target = FieldSystem.unitSystem.GetUnit(pos);
                if (target is Enemy)
                {
                    passive.EnableCondition();
                    return;
                }
            }
            
            passive.DisableCondition();
        }
    }
}