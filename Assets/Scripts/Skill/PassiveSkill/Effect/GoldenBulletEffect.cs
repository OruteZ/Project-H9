using PassiveSkill;

public class GoldenBulletEffect : BaseEffect
{
    protected int goldenBulletCount = 1;
    public GoldenBulletEffect(StatType statType, int amount) : base(statType, amount)
    {
    }

    protected override void EffectSetup()
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.GoldenBullet;
    }

    public override void OnConditionEnable()
    {
        if (unit.goldenBulletCount < goldenBulletCount) unit.goldenBulletCount = goldenBulletCount;
    }

    public override void OnConditionDisable()
    {
    }
}
public class TwoGoldenBulletsEffect : GoldenBulletEffect
{
    protected new int goldenBulletCount = 2;
    public TwoGoldenBulletsEffect(StatType statType, int amount) : base(statType, amount)
    {
    }
    public override void OnConditionEnable()
    {
        if (unit.goldenBulletCount < goldenBulletCount) unit.goldenBulletCount = goldenBulletCount;
    }
}
