namespace PassiveSkill
{
    public class TargetHighHp : BaseCondition 
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

        public TargetHighHp(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp >= amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
    
    public class TargetHpIs : BaseCondition
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

        public TargetHpIs(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp == (int)amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
    
    public class TargetLowHp : BaseCondition
    {
        public TargetLowHp(float amt) : base(amt)
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
            if(target.hp <= amount) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }

    
    public class TargetHpMax : BaseCondition
    {
        public TargetHpMax(float amt) : base(amt)
        { }
        
        public override ConditionType GetConditionType()
        {
            return ConditionType.TargetHpMax;
        }

        protected override void ConditionSetup()
        {
            unit.onStartShoot.AddListener(SetTarget);
            unit.onFinishShoot.AddListener(TargetOff);
        }

        private void SetTarget(Unit target)
        {
            if(target.hp == target.GetStat().maxHp) passive.EnableCondition();
            else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.DisableCondition();
        }
    }
}