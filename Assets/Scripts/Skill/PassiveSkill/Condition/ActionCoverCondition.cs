using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class CoveredThisTurnCondition : ActionThisTurnCondition<CoverAction>
    {
        public CoveredThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.CoveredThisTurn;
    }
    public class CoveringCondition : ActingCondition<CoverAction>
    {
        public CoveringCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Covering;
    }
    public class NotCoveredThisTurnCondition : NotActedThisTurnCondition<CoverAction>
    {
        public NotCoveredThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotCoveredThisTurn;
    }
    #endregion
}