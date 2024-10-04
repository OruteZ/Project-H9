using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedDynamiteThisTurnCondition : ActionThisTurnCondition<DynamiteAction>
    {
        public UsedDynamiteThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedDynamiteThisTurn;
    }
    public class UsingDynamiteCondition : ActingCondition<DynamiteAction>
    {
        public UsingDynamiteCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingDynamite;
    }
    public class NotUsedDynamiteThisTurnCondition : NotActedThisTurnCondition<DynamiteAction>
    {
        public NotUsedDynamiteThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedDynamiteThisTurn;
    }
    #endregion
}