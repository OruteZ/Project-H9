namespace PassiveSkill
{
    public interface IEffect
    {
        void Setup(Passive passive);
        PassiveEffectType GetEffectType();
        bool IsEnable();
        void OnConditionEnable();
        void OnConditionDisable();
    }

    public enum PassiveEffectType
    {
        Null
    }
}
