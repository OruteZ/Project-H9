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
        
        UseFanning,
        UseFanningAndCheckChance,
        HitSixFanningShot,

        ShootAGoldenBullet,
        TargetIsHitByGoldenBulletInThisTurn,
       
        TargetOnSweetSpot,
        KillEnemy,
        KillEnemyOnSweetSpot,

        Critical,
        NonCritical,

        EquipRevolver,
        EquipRepeater,
        EquipShotgun,

        //maybe not use anymore?
        Revenge,
        Dying,
        Snipe,
        Fighter
    }
}
