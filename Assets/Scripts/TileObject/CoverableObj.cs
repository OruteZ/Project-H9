using UnityEngine;
using UnityEngine.Events;

public class CoverableObj : TileObject, IDamageable
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    
    [SerializeField] private CoverType coverType;
    [SerializeField] private Unit unit;
    
    private UnityEvent<int, int> _onHpChanged = new UnityEvent<int, int>();
    
    public override void SetUp()
    {
        base.SetUp();
        _onHpChanged.RemoveAllListeners();
        
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
    
    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        
        UnityAction onDodged = () => TakeDamage(1, newUnit);
        
        
        // todo : Damage Context »ý¼º
        // newUnit.onDodged.AddListener(onDodged);
        // newUnit.onMoved.AddListener((u) =>
        // {
        //     newUnit.onDodged.RemoveListener(onDodged);
        //     unit = null;
        // });
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

    public int GetHitRateModifier(Unit attacker)
    {
        return 100000;
    }

    public UnityEvent<int, int> OnHpChanged => _onHpChanged;

    #endregion

    // Get the direction from the player to the cover
    // 
    public Vector3Int GetCoverDirection(Vector3Int playerHexPosition)
    {
        // Get the direction from the player to the cover
        return hexPosition - playerHexPosition;
    }
}

public enum CoverType
{
    LIGHT,
    HEAVY,
    NONE
}