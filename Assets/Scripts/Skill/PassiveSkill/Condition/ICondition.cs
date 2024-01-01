namespace PassiveSkill
{
    public interface ICondition
    {
        void Setup(Passive passive);
        ConditionType GetConditionType();
        void SetAmount(float amount);
        
        void OnDelete();
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
        
        Revenge,
        Dying,
        
        Snipe,
        Fighter
    }
}
