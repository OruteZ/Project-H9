namespace PassiveSkill
{
    public class LessTargetRange : BaseCondition 
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

        public LessTargetRange(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.hexPosition);
            
            if(dist <= amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
    
    public class MoreTargetRange : BaseCondition
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

        public MoreTargetRange(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            var dist = Hex.Distance(unit.hexPosition, target.hexPosition);
            
            if(dist >= (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
    
    public class SameTargetRange : BaseCondition
    {
        public SameTargetRange(float amt) : base(amt)
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
            
            if(dist == (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
}