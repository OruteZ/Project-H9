using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;

public class CoverableObj : TileObject, IDamageable
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    
    [SerializeField] private CoverType coverType;
    [SerializeField] private Unit unit;
    
    private readonly UnityEvent<int, int> _onHpChanged = new UnityEvent<int, int>();
    
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

        newUnit.onHit.AddListener(OnHit);
        newUnit.onMoved.AddListener(OnUnitMoved);
    }

    private void OnHit(Damage context)
    {
        // set gizmo
        gizmoFlag = true;
        g_targetHex = context.target.GetHex();
        g_atkFromHex = context.attacker.GetHex();
        
        
        if (!context.Contains(Damage.Type.MISS)) return;
        
        bool isCovered = Coverable(context.attacker.GetHex(), hexPosition, context.target.GetHex());
        if (!isCovered) return;
        

        Damage selfDamage = new (1, 1, Damage.Type.DEFAULT, context.attacker, this);
        TakeDamage(selfDamage);
    }

    private void OnUnitMoved(Unit u)
    {
        u.onHit.RemoveListener(OnHit);
        u.onMoved.RemoveListener(OnUnitMoved);
        unit = null;
    }

    #region IDamageable
    public void TakeDamage(Damage damage)
    {
        currentHp -= damage.GetFinalAmount();
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
    public Vector3Int GetCoverDirection(Vector3Int playerHexPosition)
    {
        // Get the direction from the player to the cover
        return hexPosition - playerHexPosition;
    }

    public static bool Coverable(Vector3Int atkFrom, Vector3Int coverObjPos, Vector3Int targetPos)
    {
        
        Vector3 target = Hex.Hex2World(targetPos) + Vector3.up;
        Vector3 cover = Hex.Hex2World(coverObjPos) + Vector3.up;
        Vector3 atkFromWorld = Hex.Hex2World(atkFrom) + Vector3.up;
        
        Vector3 midVector = (cover - target).normalized;
        Vector3 coverToAtkFrom = (atkFromWorld - cover).normalized;
        
        // is coverToAtkFrom between cwVector and ccwVector?
        float angle = Vector3.Angle(midVector, coverToAtkFrom);
        if (angle <= 60)
        {
            return true;
        }
            
        return false;
    }

    private bool gizmoFlag = false;
    private Vector3Int g_targetHex;
    private Vector3Int g_atkFromHex;

    public void OnDrawGizmos()
    {
        if (!gizmoFlag) return;
        
        // show 2 vectors
        Vector3 target = Hex.Hex2World(g_targetHex) + Vector3.up;
        Vector3 coverPos = Hex.Hex2World(hexPosition) + Vector3.up;
        Vector3 atkFromPos = Hex.Hex2World(g_atkFromHex) + Vector3.up;
        
        Vector3 midVector = (coverPos - target).normalized;
        Vector3 cwVector = Quaternion.AngleAxis(60, Vector3.up) * midVector;
        Vector3 ccwVector = Quaternion.AngleAxis(-60, Vector3.up) * midVector;
        Vector3 coverToAtkFrom = (atkFromPos - coverPos).normalized;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(coverPos, coverPos + midVector * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(coverPos, coverPos + cwVector * 10);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(coverPos, coverPos + ccwVector * 10);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(coverPos, coverPos + coverToAtkFrom * 10);
    }
}

public enum CoverType
{
    LIGHT,
    HEAVY,
    NONE
}