using UnityEngine;

public class CoverableObj : TileObject, IDamageable
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    
    [SerializeField] private CoverType coverType;
    
    public override void SetUp()
    {
        base.SetUp();
        
        currentHp = maxHp;
    }
    
    public override string[] GetArgs()
    {
        return new[]
        {
            maxHp.ToString(),
            currentHp.ToString()
        };
    }

    public override void SetArgs(string[] args)
    {
        if (args.Length != 2) throw new System.Exception("Invalid args length. Expected 2.");
        
        maxHp = int.Parse(args[0]);
        currentHp = int.Parse(args[1]);
    }
    
    public CoverType GetCoverType()
    {
        return coverType;
    }

    #region IDamageable
    public void TakeDamage(int damage, Unit attacker, Damage.Type type = Damage.Type.Default)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            
            // remove obj
            RemoveSelf();
        }
    }

    public Vector3Int GetHex()
    {
        return hexPosition;
    }

    public int GetCurrentHp()
    {
        return currentHp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public int GetHitRateModifier()
    {
        return 0;
    }

    #endregion
}

public enum CoverType
{
    Light,
    Heavy,
    None
}