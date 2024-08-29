using UnityEngine;

namespace PassiveSkill
{
    public class CriticalShotCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.Critical;

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public CriticalShotCondition(float amt) : base(amt)
        { }

        private void SetTarget(IDamageable target)
        {
            passive.NotFullfillCondition(this);
        }

        private void TargetOff(Damage context)
        {
            if (context.Contains(Damage.Type.CRITICAL))
            {
                passive.FullfillCondition(this);
            }
        }
    }
    public class NonCriticalShotCondition : BaseCondition
    {
        public override ConditionType GetConditionType() => ConditionType.NonCritical;

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public NonCriticalShotCondition(float amt) : base(amt)
        { }

        private void SetTarget(IDamageable target)
        {
            passive.NotFullfillCondition(this);
        }

        private void TargetOff(Damage context)
        {
            if (context.Contains(Damage.Type.CRITICAL) is false)
            {
                passive.FullfillCondition(this);
            }
        }
    }
}
