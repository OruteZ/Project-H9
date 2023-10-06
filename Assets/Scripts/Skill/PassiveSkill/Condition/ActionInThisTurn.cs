using Unity.VisualScripting;

namespace PassiveSkill
{
    public class MovedInThisTurn : BaseCondition
    {
        public MovedInThisTurn(float amt) : base(amt)
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
            if (unitAction is MoveAction) passive.EnableCondition();
        }

        private void ClearFlag()
        {
            passive.DisableCondition();
        }
    }
    
    public class NotMovedInThisTurn : BaseCondition
    {
        public NotMovedInThisTurn(float amt) : base(amt)
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
            if (unitAction is MoveAction) passive.DisableCondition();
        }

        private void ClearFlag()
        {
            if (FieldSystem.turnSystem.turnOwner == unit)
            {
                passive.EnableCondition();
            }
        }
    }
    
    
    public class ReloadedInThisTurn : BaseCondition
    {
        public ReloadedInThisTurn(float amt) : base(amt)
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
            if (unitAction is ReloadAction) passive.EnableCondition();
        }

        private void ClearFlag()
        {
            passive.DisableCondition();
        }
    }
}