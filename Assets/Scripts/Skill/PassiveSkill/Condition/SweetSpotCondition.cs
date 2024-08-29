using UnityEngine;

namespace PassiveSkill
{
    public class TargetOnSweetSpotCondition : BaseCondition
    {
        public TargetOnSweetSpotCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.TargetOnSweetSpot;

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
            unit.onKill.AddListener(Debug.LogError);
        }

        private void SetTarget(IDamageable target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.GetHex());
            if (unit.weapon is not Repeater repeater) return;

            if (dist == repeater.GetSweetSpot()) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }

        private void TargetOff(Damage context)
        {
            passive.NotFullfillCondition(this);
        }
    }
    public class KillOnSweetSpotCondition : BaseCondition
    {
        public KillOnSweetSpotCondition(float amt) : base(amt)
        { }

        public override ConditionType GetConditionType() => ConditionType.KillEnemyOnSweetSpot;

        IDamageable _killTarget;
        protected override void ConditionSetup()
        {
            _killTarget = null;
            unit.onStartShoot.AddListener(SetTarget);
            unit.onKill.AddListener(OnKill);
        }

        private void SetTarget(IDamageable target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.GetHex());
            if (unit.weapon is not Repeater repeater) return;

            if (dist == repeater.GetSweetSpot())
            {
                _killTarget = target;
            }
            else
            {
                _killTarget = null;
            }
        }
        protected void OnKill(Unit target)
        {
            if (target == (Unit)_killTarget)
            {
                passive.FullfillCondition(this);
            }
            else
            {
                passive.NotFullfillCondition(this);
            }
        }
    }
}

