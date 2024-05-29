public struct Damage
{
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
        Default = 1 << 0,
        Critical = 1 << 1,
        Blooded = 1 << 2,
        Burned = 1 << 3,
        Heal = 1 << 4,
        Miss = 0,
    };
    
    public bool Contains(Type type, Type value)
    {
        return (type & value) == value;
    }

    private int amount;
    private Unit attacker;
    private Unit target;
}
