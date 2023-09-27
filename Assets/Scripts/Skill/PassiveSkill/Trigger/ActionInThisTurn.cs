using Unity.VisualScripting;

namespace PassiveSkill
{
    public class MovedInThisTurn : BaseTrigger
    {
        public MovedInThisTurn(float amt) : base(amt)
        { }

        public override TriggerType GetTriggerType()
        {
            return TriggerType.MovedInThisTurn;
        }

        protected override void TriggerSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.TurnOnPassive();
        }

        private void ClearFlag()
        {
            passive.TurnOffPassive();
        }
    }
    
    public class NotMovedInThisTurn : BaseTrigger
    {
        public NotMovedInThisTurn(float amt) : base(amt)
        { }

        public override TriggerType GetTriggerType()
        {
            return TriggerType.NotMovedInThisTurn;
        }

        protected override void TriggerSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.TurnOffPassive();
        }

        private void ClearFlag()
        {
            if (FieldSystem.turnSystem.turnOwner == unit)
            {
                passive.TurnOnPassive();
            }
        }
    }
    
    
    public class ReloadedInThisTurn : BaseTrigger
    {
        public ReloadedInThisTurn(float amt) : base(amt)
        { }

        public override TriggerType GetTriggerType()
        {
            return TriggerType.ReloadedInThisTurn;
        }

        protected override void TriggerSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }

        //---
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ReloadAction) passive.TurnOnPassive();
        }

        private void ClearFlag()
        {
            passive.TurnOffPassive();
        }
    }
}