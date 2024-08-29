using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedHemostasisThisTurnCondition : BaseCondition
    {
        public UsedHemostasisThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedHemostasisThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is HemostasisAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class UsingHemostasisCondition : BaseCondition
    {
        public UsingHemostasisCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingHemostasis;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is HemostasisAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is HemostasisAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotUsedHemostasisThisTurnCondition : BaseCondition
    {
        public NotUsedHemostasisThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedHemostasisThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is HemostasisAction) passive.NotFullfillCondition(this);
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