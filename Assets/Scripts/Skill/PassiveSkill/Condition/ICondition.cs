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
        EquipRevolver,
        EquipRepeater,
        EquipShotgun,
        KillEnemy,
        FailToKillEnemy,
        Critical,
        NonCritical,
        HitSixFanningShot,
        null1,
        null2,
        null3,
        ShootAGoldenBullet,
        TargetIsHitedByGoldenBulletThisTurn,
        null4,
        null5,
        null6,
        TargetOnSweetSpot,
        KillEnemyOnSweetSpot,
        null7,
        null8,
        null9,
        null10,
        null11,
        null12,
        null13,
        null14,
        NotMovedThisTurn,
        Moving,
        MovedThisTurn,
        NotAttackedThisTurn,
        Attacking,
        AttackedThisTurn,
        NotReloadedThisTurn,
        Reloading,
        ReloadedThisTurn,
        NotCoveredThisTurn,
        Covering,
        CoveredThisTurn,
        NotUsedItemThisTurn,
        UsingItem,
        UsedItemThisTurn,
        NotUsedFanningThisTurn,
        UsingFanning,
        UsedFanningThisTurn,
        NotUsedDynamiteThisTurn,
        UsingDynamite,
        UsedDynamiteThisTurn,
        NotUsedHemostasisThisTurn,
        UsingHemostasis,
        UsedHemostasisThisTurn,

        //maybe not use anymore?
        Revenge,
        Dying,
        Snipe,
        Fighter,
        UseDynamite,
    }
}
