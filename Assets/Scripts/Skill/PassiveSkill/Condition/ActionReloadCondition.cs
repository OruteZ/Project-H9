using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class ReloadedThisTurnCondition : BaseCondition
    {
        public ReloadedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.ReloadedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ReloadAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class ReloadingCondition : BaseCondition
    {
        public ReloadingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Reloading;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is ReloadAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is ReloadAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotReloadedThisTurnCondition : BaseCondition
    {
        public NotReloadedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotReloadedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ReloadAction) passive.NotFullfillCondition(this);
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