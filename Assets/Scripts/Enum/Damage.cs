using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public readonly struct Damage
{
    public static uint index = 0;

    public readonly uint id;
    public readonly int amount;
    public readonly int criticalAmount;
    public readonly Type type;
    public readonly Unit attacker;
    public readonly IDamageable target;
    
    // constructor
    public Damage(
        int amount, 
        int criticalAmount,
        Type type, 
        Unit attacker,
        IDamageable target
        )
    
    {
        id = index++;
        this.amount = amount;
        this.criticalAmount = criticalAmount;
        this.type = type;
        this.attacker = attacker;
        this.target = target;
    }
    
    
    public bool Contains(Type value)
    {
        return (type & value) == value;
    }
    public int GetFinalAmount()
    {
        if (Contains(Type.CRITICAL)) return criticalAmount;
        if (Contains(Type.MISS)) return 0;

        return amount;
    }
    
    /// <summary>
    /// 비트연산으로 동작하는 DamageType
    /// </summary>
    /// <comment>
    /// Damage Type을 나타내어 크리티컬, 출혈, 화상 등의 종류를 사용해 DamageFloater를 변경하기 위한 코드
    /// 만약 데미지의 종류를 별개 나누어 "이 캐릭터는 화상데미지를 2 더 받습니다." 따위를 만들러온 당신,
    /// Ciritical은 별개의 변수로 빼어 "화상데미지는 크리티컬 데미지가 따로 있을까요"도 여쭙고 사용하길 바람
    /// enum값이 아닌 DamageType 클래스 자체를 하나 생성하는 것이 기본적으로는 괜찮아보임
    ///     (데미지의 타입 등을 이용해 계산 자체나 Log에 적재하기 위해)
    /// </comment>
    [System.Flags]
    public enum Type { 
        MISS = 1 << 0,
        DEFAULT = 1 << 1,
        CRITICAL = 1 << 2,
        BLOODED = 1 << 3,
        BURNED = 1 << 4,
        HEAL = 1 << 5,
        
        // 하인리히 보스의 은신을 벗겨내기 위한 데미지 타입이지만, 이후 은신을 밝혀내는 데미지 특성이 될 지도 모름
        UNVANISHABLE = 1 << 6,
        
    };
}
