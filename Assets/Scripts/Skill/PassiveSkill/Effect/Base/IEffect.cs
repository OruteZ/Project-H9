namespace PassiveSkill
{
    public interface IEffect
    {
        void Setup(Passive passive);
        PassiveEffectType GetEffectType();
        bool IsEnable();
        void OnConditionEnable();
        void OnConditionDisable();
        void OnDelete();
    }
    

    public enum PassiveEffectType
    {
        Null,
        IncreaseStatByCondition,
        IncreaseStatDuringAction,
        IncreaseStatForOneTurn,
        IncreaseStatForTwoTurns,
        IncreaseStatForThreeTurns,
        IncreaseStatForFourTurns,
        IncreaseStatForFiveTurns,
        null1,
        null2,
        DoubleShootPoint,
        InfinityShootPoint,
        null3,
        null4,
        null5,
        LightFootStep,
        FreeReload,
        GoldenBullet,
        SweetSpot,
        DemolitionDynamite,
    }
}
