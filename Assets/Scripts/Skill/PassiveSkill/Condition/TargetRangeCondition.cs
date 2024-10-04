using UnityEngine;

namespace PassiveSkill
{
    public class LessTargetRangeCondition : BaseCondition
    {
        public LessTargetRangeCondition(float amt) : base(amt) { }
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
            var dist = Hex.Distance(unit.hexPosition, target.GetHex());
            
            if(dist <= amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
    
    public class MoreTargetRangeCondition : BaseCondition
    {
        public MoreTargetRangeCondition(float amt) : base(amt) { }
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
            var dist = Hex.Distance(unit.hexPosition, target.GetHex());
            
            if(dist >= (int)amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
    
    public class SameTargetRangeCondition : BaseCondition
    {
        public SameTargetRangeCondition(float amt) : base(amt) { }
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
            var dist = Hex.Distance(unit.hexPosition, target.GetHex());
            
            if(dist == (int)amount) passive.FullfillCondition(this);
            else passive.NotFullfillCondition(this);
        }
        private void TargetOff()
        {
            passive.NotFullfillCondition(this);
        }
    }
}