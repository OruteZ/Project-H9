namespace PassiveSkill
{
    public interface ICondition
    {
        void Setup(Passive passive);
        ConditionType GetConditionType();
        void SetAmount(float amount);
    }

    public enum ConditionType
    {
        Null,
        TargetHpMax,
        TargetLowHp,
        TargetHighHp,
        TargetHpIs,
        LowAmmo,
        HighAmmo,
        AmmoIs,
        
        LessTargetRange,
        MoreTargetRange,
        SameTargetRange,
        
        LowHp,
        HighHp,
        HpIs,
        ReloadedInThisTurn,
        MovedInThisTurn,
        NotMovedInThisTurn,
    }
}
