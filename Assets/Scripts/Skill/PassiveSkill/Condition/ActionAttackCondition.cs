using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class AttackedThisTurnCondition : ActionThisTurnCondition<AttackAction>
    {
        public AttackedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.AttackedThisTurn;
    }
    public class AttackingCondition : ActingCondition<AttackAction>
    {
        public AttackingCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.Attacking;
    }
    public class NotAttackedThisTurnCondition : NotActedThisTurnCondition<AttackAction>
    {
        public NotAttackedThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotAttackedThisTurn;
    }
    #endregion
}