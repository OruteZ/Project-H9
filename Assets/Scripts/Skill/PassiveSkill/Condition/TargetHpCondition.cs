namespace PassiveSkill
{
    public class TargetHighHpCondition : BaseCondition 
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

        public TargetHighHpCondition(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp >= amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
    
    public class TargetHpIsCondition : BaseCondition
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

        public TargetHpIsCondition(float amt) : base(amt)
        { }

        private void SetTarget(Unit target)
        {
            if(target.hp == (int)amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
    
    public class TargetLowHpCondition : BaseCondition
    {
        public TargetLowHpCondition(float amt) : base(amt)
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
            if(target.hp <= amount) passive.Enable();
            else passive.Disable();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }

    
    public class TargetHpMaxCondition : BaseCondition
    {
        public TargetHpMaxCondition(float amt) : base(amt)
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
            if(target.hp == target.stat.GetStat(StatType.MaxHp)) passive.Enable();
            // else passive.DisableCondition();
        }

        private void TargetOff(Unit target, int damage, bool none, bool __)
        {
            passive.Disable();
        }
    }
}