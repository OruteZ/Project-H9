using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedDynamiteThisTurnCondition : BaseCondition
    {
        public UsedDynamiteThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedDynamiteThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is DynamiteAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class UsingDynamiteCondition : BaseCondition
    {
        public UsingDynamiteCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingDynamite;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is DynamiteAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is DynamiteAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotUsedDynamiteThisTurnCondition : BaseCondition
    {
        public NotUsedDynamiteThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedDynamiteThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is DynamiteAction) passive.NotFullfillCondition(this);
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