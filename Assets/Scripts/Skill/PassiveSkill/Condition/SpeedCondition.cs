namespace PassiveSkill
{
    public class HighSpeedCondition : BaseCondition
    {
        public HighSpeedCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.HighSpeed;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable t)
        {
            if (t is not Unit) return;
            Unit target = (Unit)t;

            if (target.stat.speed < unit.stat.speed) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }

    public class LowSpeedCondition : BaseCondition
    {
        public LowSpeedCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.LowSpeed;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable t)
        {
            if (t is not Unit) return;
            Unit target = (Unit)t;

            if (target.stat.speed > unit.stat.speed) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
}