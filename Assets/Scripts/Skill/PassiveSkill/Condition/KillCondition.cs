using UnityEngine;

namespace PassiveSkill
{
    public class KillCondition : BaseCondition
    {
        public KillCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.KillEnemy;

        protected override void ConditionSetup()
        {
            unit.onKill.AddListener(OnKill);
        }

        protected void OnKill(Unit target)
        {
            passive.FullfillCondition(this);
        }
    }
    public class FailToKillCondition : BaseCondition
    {
        IDamageable _target;
        public FailToKillCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.FailToKillEnemy;

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(CheckCondition);
        }

        private void SetTarget(IDamageable target)
        {
            if (target.GetCurrentHp() < 0) return;
            passive.NotFullfillCondition(this);
            _target = target;
        }

        private void CheckCondition(Damage context)
        {
            if (_target == null || _target.GetCurrentHp() < 0) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
    }
}


