using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedItemThisTurnCondition : ActionThisTurnCondition<ItemUsingAction>
    {
        public UsedItemThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedItemThisTurn;
    }
    public class UsingItemCondition : ActingCondition<ItemUsingAction>
    {
        public UsingItemCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingItem;
    }
    public class NotUsedItemThisTurnCondition : NotActedThisTurnCondition<ItemUsingAction>
    {
        public NotUsedItemThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedItemThisTurn;
    }
    #endregion
}