namespace PassiveSkill
{
    public interface IEffect
    {
        void Setup(Passive passive);
        PassiveEffectType GetEffectType();

        void Enable();
        void Disable();
    }

    public enum PassiveEffectType
    {
        RevolverAdditionalDamageUp,
    }
}
