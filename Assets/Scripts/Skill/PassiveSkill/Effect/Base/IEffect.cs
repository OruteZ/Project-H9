namespace PassiveSkill
{
    public interface IEffect
    {
        void Setup(Passive passive);
        PassiveEffectType GetEffectType();

        void Enable();
        void Disable();
    }

    public enum PassiveEffectType
    {
        Null,
        HpUp,
        ConcentrationUp,
        SightRangeUp,
        SpeedUp,
        ActionPointUp,
        AdditionalHitRateUp,
        CriticalChanceUp,
        RevolverAdditionalDamageUp,
        RevolverAdditionalRangeUp,
        RevolverCriticalDamageUp,
        RepeaterAdditionalDamageUp,
        RepeaterAdditionalRangeUp,
        RepeaterCriticalDamageUp,
        ShotgunAdditionalDamageUp,
        ShotgunAdditionalRangeUp,
        ShotgunCriticalDamageUp,
    }
}
