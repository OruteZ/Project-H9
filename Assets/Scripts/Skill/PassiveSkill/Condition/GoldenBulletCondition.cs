using UnityEngine;

namespace PassiveSkill
{
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

    public class TargetIsHitedByGoldenBulletThisTurn : BaseCondition
    {
        public TargetIsHitedByGoldenBulletThisTurn(float amt) : base(amt)
        { }
        public override ConditionType GetConditionType() => ConditionType.TargetIsHitedByGoldenBulletThisTurn;

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(CheckTarget);
            unit.onFinishAction.AddListener((a) => { passive.NotFullfillCondition(this); });
        }

        private void CheckTarget(IDamageable dmgable)
        {
            if (dmgable is Unit unit && unit.isHitedByGoldenBulletThisTurn) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
    }
}
