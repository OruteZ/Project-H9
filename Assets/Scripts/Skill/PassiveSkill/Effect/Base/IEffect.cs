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
        StatUpDependedOnCondition,
        LightFootStep,
        InfinityShootPoint,
        TwoShootPoint,
        FreeReload,
        GoldenBullet,
        TwoGoldenBullet,
        StatUpDuringThreeTurn,
        SweetSpot,
        StatUpWhileAction
    }
}
