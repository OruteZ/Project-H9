namespace PassiveSkill
{
    public class MovedInThisTurnCondition : BaseCondition
    {
        public MovedInThisTurnCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType()
        {
            return ConditionType.MovedInThisTurn;
        }

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.FullfillCondition(this);
        }

        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    
    public class NotMovedInThisTurnCondition : BaseCondition
    {
        public NotMovedInThisTurnCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType()
        {
            return ConditionType.NotMovedInThisTurn;
        }

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.NotFullfillCondition(this);
        }

        private void ClearFlag()
        {
            if (FieldSystem.turnSystem.turnOwner == unit)
            {
                passive.FullfillCondition(this);
            }
        }
    }
    
    
    public class ReloadedInThisTurnCondition : BaseCondition
    {
        public ReloadedInThisTurnCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType()
        {
            return ConditionType.ReloadedInThisTurn;
        }

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ReloadAction) passive.FullfillCondition(this);
        }

        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
}