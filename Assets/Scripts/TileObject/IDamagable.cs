using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    void TakeDamage(int damage, Unit attacker, Damage.Type type = Damage.Type.Default);
    Vector3Int GetHex();
    int GetCurrentHp();
    int GetMaxHp();
    int GetHitRateModifier();
    
    UnityEvent<int, int> OnHpChanged { get; }
}