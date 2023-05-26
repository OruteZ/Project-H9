using UnityEngine;

public class CriticalDoubleTab : Gimmick
{
    protected override void OnCriticalAttack(Unit target, int damage)
    {
        target.OnHit(damage);
    }
}
