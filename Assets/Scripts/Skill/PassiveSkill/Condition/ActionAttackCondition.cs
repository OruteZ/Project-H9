using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class AttackedThisTurnCondition : BaseCondition
    {
        public AttackedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.AttackedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is AttackAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class AttackingCondition : BaseCondition
    {
        public AttackingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Attacking;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is AttackAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is AttackAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotAttackedThisTurnCondition : BaseCondition
    {
        public NotAttackedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotAttackedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is AttackAction) passive.NotFullfillCondition(this);
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