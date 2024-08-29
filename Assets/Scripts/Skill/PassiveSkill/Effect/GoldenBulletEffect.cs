using PassiveSkill;

public class GoldenBulletEffect : BaseEffect
{
    protected bool isEffectOn = false;
    public GoldenBulletEffect(StatType statType, int amount) : base(statType, amount) { }
    protected override void EffectSetup() { }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.GoldenBullet;
    }

    public override void OnConditionEnable()
    {
        if (!isEffectOn) 
        {
            isEffectOn = true;
            unit.goldenBulletChance += GetAmount();
        }
    }
    public override void OnConditionDisable()
    {
        if (isEffectOn)
        {
            isEffectOn = false;
            unit.goldenBulletChance -= GetAmount();
        }
    }
}
public class TwoGoldenBulletsEffect : GoldenBulletEffect
{
    protected new bool isEffectOn = false;
    public TwoGoldenBulletsEffect(StatType statType, int amount) : base(statType, amount) { }
    public override void OnConditionEnable()
    {
        if (!isEffectOn)
        {
            isEffectOn = true;
            unit.goldenBulletChance += GetAmount();
        }
    }
    public override void OnConditionDisable()
    {
        if (isEffectOn)
        {
            isEffectOn = false;
            unit.goldenBulletChance -= GetAmount();
        }
    }
}
public class ThreeGoldenBulletsEffect : GoldenBulletEffect
{
    protected new bool isEffectOn = false;
    public ThreeGoldenBulletsEffect(StatType statType, int amount) : base(statType, amount) { }
    public override void OnConditionEnable()
    {
        if (!isEffectOn)
        {
            isEffectOn = true;
            unit.goldenBulletChance += GetAmount();
        }
    }
    public override void OnConditionDisable()
    {
        if (isEffectOn)
        {
            isEffectOn = false;
            unit.goldenBulletChance -= GetAmount();
        }
    }
}
