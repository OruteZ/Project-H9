namespace PassiveSkill
{
    public class LessTargetRangeCondition : BaseCondition 
    {
        public override ConditionType GetConditionType()
        {
            return ConditionType.TargetHighHp;
        }

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public LessTargetRangeCondition(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.hexPosition);
            
            if(dist <= amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
    
    public class MoreTargetRangeCondition : BaseCondition
    {
        public override ConditionType GetConditionType()
        {
            return ConditionType.TargetHpIs;
        }

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        public MoreTargetRangeCondition(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.hexPosition);
            
            if(dist >= (int)amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
    
    public class SameTargetRangeCondition : BaseCondition
    {
        public SameTargetRangeCondition(float amt) : base(amt)
        { }
        
        public override ConditionType GetConditionType()
        {
            return ConditionType.TargetLowHp;
        }

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        private void SetTarget(Unit target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.hexPosition);
            
            if(dist == (int)amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
}