using UnityEngine;

namespace PassiveSkill
{
    #region ActionBaseConditions
    public class UsedHemostasisThisTurnCondition : ActionThisTurnCondition<HemostasisAction>
    {
        public UsedHemostasisThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsedHemostasisThisTurn;
    }
    public class UsingHemostasisCondition : ActingCondition<HemostasisAction>
    {
        public UsingHemostasisCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.UsingHemostasis;
    }
    public class NotUsedHemostasisThisTurnCondition : NotActedThisTurnCondition<HemostasisAction>
    {
        public NotUsedHemostasisThisTurnCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.NotUsedHemostasisThisTurn;
    }
    #endregion
}