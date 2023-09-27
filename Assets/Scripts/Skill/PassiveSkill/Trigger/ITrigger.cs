namespace PassiveSkill
{
    public interface ITrigger
    {
        void Setup(Passive passive);
        TriggerType GetTriggerType();
        void SetAmount(float amount);
    }

    public enum TriggerType
    {
        Null,
        TargetLowHp,
        TargetHighHp,
        TargetHpIs,
        LowAmmo,
        HighAmmo,
        AmmoIs,
        LowHp,
        HighHp,
        HpIs,
        ReloadedInThisTurn,
        MovedInThisTurn,
        NotMovedInThisTurn,
    }
}
