using PassiveSkill;

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

    private void SetTarget(Unit target)
    {
        passive.NotFullfillCondition(this);
    }

    private void TargetOff(Unit target, int damage, bool isHit, bool isCritical)
    {
        if (isCritical)
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

    private void SetTarget(Unit target)
    {
        passive.NotFullfillCondition(this);
    }

    private void TargetOff(Unit target, int damage, bool isHit, bool isCritical)
    {
        if (!isCritical)
        {
            passive.FullfillCondition(this);
        }
    }
}
