using PassiveSkill;
using UnityEngine;

public class ShootAGoldenBulletCondition : BaseCondition
{
    public ShootAGoldenBulletCondition(float amt) : base(amt)
    { }
    public override ConditionType GetConditionType() => ConditionType.ShootAGoldenBullet;

    protected override void ConditionSetup()
    {
        unit.onActionStart.AddListener(CheckBullet);
        unit.onFinishAction.AddListener((a) => { passive.NotFullfillCondition(this); });
    }

    private void CheckBullet(IUnitAction action, Vector3Int pos)
    {
        if (unit.weapon.GetWeaponType() != ItemType.Revolver || action is not AttackAction || !unit.weapon.magazine.GetNextBullet().isGoldenBullet) return;
        passive.FullfillCondition(this);
    }
}

public class TargetIsHitByGoldenBulletInThisTurn : BaseCondition
{
    public TargetIsHitByGoldenBulletInThisTurn(float amt) : base(amt)
    { }
    public override ConditionType GetConditionType() => ConditionType.TargetIsHitByGoldenBulletInThisTurn;

    protected override void ConditionSetup()
    {
        unit.onStartShoot.AddListener(CheckTarget);
        unit.onFinishAction.AddListener((a) => { passive.NotFullfillCondition(this); });
    }

    private void CheckTarget(Unit unit)
    {
        passive.FullfillCondition(this);
    }
}
