using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class ReloadedThisTurnCondition : ActionThisTurnCondition<ReloadAction>
    {
        public ReloadedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.ReloadedThisTurn;
    }
    public class ReloadingCondition : ActingCondition<ReloadAction>
    {
        public ReloadingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Reloading;
    }
    public class NotReloadedThisTurnCondition : NotActedThisTurnCondition<ReloadAction>
    {
        public NotReloadedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotReloadedThisTurn;
    }
    #endregion
}