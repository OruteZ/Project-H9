using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class CoveredThisTurnCondition : BaseCondition
    {
        public CoveredThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.CoveredThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is CoverAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class CoveringCondition : BaseCondition
    {
        public CoveringCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Covering;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is CoverAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is CoverAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotCoveredThisTurnCondition : BaseCondition
    {
        public NotCoveredThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotCoveredThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is CoverAction) passive.NotFullfillCondition(this);
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