using UnityEngine;

namespace PassiveSkill
{
    public class TargetHighHpCondition : BaseCondition
    {
        public TargetHighHpCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.TargetHighHp;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable target)
        {
            if(target.GetCurrentHp() >= amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
    
    public class TargetHpIsCondition : BaseCondition
    {
        public TargetHpIsCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.TargetHpIs;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable target)
        {
            if(target.GetCurrentHp() == (int)amount)  passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
    
    public class TargetLowHpCondition : BaseCondition
    {
        public TargetLowHpCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.TargetLowHp;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable target)
        {
            if(target.GetCurrentHp() <= amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }

    
    public class TargetHpMaxCondition : BaseCondition
    {
        public TargetHpMaxCondition(float amt) : base(amt) { }
        public override ConditionType GetConditionType() => ConditionType.TargetHpMax;

        protected override void ConditionSetup()
        {
            unit.OnAimStart.AddListener((a, p) => SetTarget(FieldSystem.unitSystem.GetUnit(p)));
            unit.OnAimEnd.AddListener((a) => TargetOff());

            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener((a) => TargetOff());
        }

        private void SetTarget(IDamageable target)
        {
            if(target.GetCurrentHp() == target.GetMaxHp()) passive.FullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
}