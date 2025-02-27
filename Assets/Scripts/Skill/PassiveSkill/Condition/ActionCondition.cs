using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions

    public abstract class ActionThisTurnCondition<Action> : BaseCondition
    {
        public ActionThisTurnCondition(float amt) : base(amt) { }
        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(ClearFlag);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is Action) passive.FullfillCondition(this);
        }
        private void ClearFlag()
        {
            passive.NotFullfillCondition(this);
        }
    }
    public abstract class ActingCondition<Action> : BaseCondition
    {
        public ActingCondition(float amt) : base(amt) { }

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener(StartAction);
            unit.OnAimEnd.AddListener(EndAction);

            unit.onActionStart.AddListener(StartAction);
            unit.onFinishAction.AddListener(EndAction);
        }
        protected void StartAction(IUnitAction unitAction, Vector3Int pos)
        {
            //Debug.LogError("add: " + unitAction);
            if (unitAction is Action) passive.FullfillCondition(this);
        }
        protected void EndAction(IUnitAction unitAction)
        {
            //Debug.LogError("cancel: "+ unitAction);
            if (unitAction is Action) passive.NotFullfillCondition(this);
        }
    }
    public abstract class NotActedThisTurnCondition<Action> : BaseCondition
    {
        public NotActedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotAttackedThisTurn;

        protected override void ConditionSetup()
        {
            unit.onFinishAction.AddListener(CheckAction);
            FieldSystem.turnSystem.onTurnChanged.AddListener(CheckTurnOwner);
        }
        private void CheckAction(IUnitAction unitAction)
        {
            if (unitAction is Action) passive.NotFullfillCondition(this);
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