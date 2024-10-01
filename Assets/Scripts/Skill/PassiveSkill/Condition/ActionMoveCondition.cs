using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class MovedThisTurnCondition : ActionThisTurnCondition<MoveAction>
    {
        public MovedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.MovedThisTurn;
    }
    public class MovingCondition : ActingCondition<MoveAction>
    {
        public MovingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Moving;
    }
    public class NotMovedThisTurnCondition : NotActedThisTurnCondition<MoveAction>
    {
        public NotMovedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotMovedThisTurn;
    }
    #endregion
}