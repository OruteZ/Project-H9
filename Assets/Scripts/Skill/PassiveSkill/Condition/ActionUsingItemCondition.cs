using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedItemThisTurnCondition : BaseCondition
    {
        public UsedItemThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedItemThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ItemUsingAction) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class UsingItemCondition : BaseCondition
    {
        public UsingItemCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingItem;

        protected override void ConditionSetup()
        {
            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            if (unitAction is ItemUsingAction) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            if (unitAction is ItemUsingAction) passive.NotFullfillCondition(this);
        }
    }
    public class NotUsedItemThisTurnCondition : BaseCondition
    {
        public NotUsedItemThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedItemThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is ItemUsingAction) passive.NotFullfillCondition(this);
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