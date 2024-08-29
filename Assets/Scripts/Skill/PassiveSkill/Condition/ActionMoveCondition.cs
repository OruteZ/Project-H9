using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class MovedThisTurnCondition : BaseCondition
    {
        public MovedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.MovedThisTurn;
        
        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class MovingCondition : BaseCondition
    {
        public MovingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Moving;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is MoveAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotMovedThisTurnCondition : BaseCondition
    {
        public NotMovedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotMovedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is MoveAction) passive.NotFullfillCondition(this);
        }
        private void CheckTurnOwner()
        {
            if (FieldSystem.turnSystem.turnOwner == unit)
            {
                passive.FullfillCondition(this);
            }
        }
    }
    #endregion
}