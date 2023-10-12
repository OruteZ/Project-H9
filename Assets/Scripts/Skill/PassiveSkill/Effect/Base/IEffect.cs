namespace PassiveSkill
{
    public interface IEffect
    {
        void Setup(Passive passive);
        PassiveEffectType GetEffectType();
        bool IsEnable();
        void OnConditionEnable();
        void OnConditionDisable();
    }

    public enum PassiveEffectType
    {
        Null,
        StatUpDependedOnCondition,
    }

    public enum StatUpType
    {
        Null,
        Hp,
        Concentration,
        SightRange,
        Speed,
        ActionPoint,
        AdditionalHitRate,
        CriticalChance,
        RevolverAdditionalDamage,
        RevolverAdditionalRange,
        RevolverCriticalDamage,
        RepeaterAdditionalDamage,
        RepeaterAdditionalRange,
        RepeaterCriticalDamage,
        ShotgunAdditionalDamage,
        ShotgunAdditionalRange,
        ShotgunCriticalDamage,
    }
}
