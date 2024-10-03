using System;
using System.Linq;
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
    [SerializeField] private Hex.Direction coverDirection;

    private static Material LightCoverMaterial => TileEffectManager.instance.combatFowMaterial;
    private bool _visible;
    
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
            currentHp.ToString(),
            coverDirection.ToString()
        };
    }

    public override void SetArgs(string[] args)
    {
        if (args.Length != 3) throw new Exception("Invalid args length. Expected 2.");
        
        maxHp = int.Parse(args[0]);
        currentHp = int.Parse(args[1]);
        coverDirection = (Hex.Direction) Enum.Parse(typeof(Hex.Direction), args[2]);
        
        OnValidate();
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
        newUnit.SetCoverType(coverType, this);
    }

    private void OnHit(Damage context)
    {
        // set gizmo
        gizmoFlag = true;
        g_targetHex = context.target.GetHex();
        g_atkFromHex = context.attacker.GetHex();
        
        
        if (!context.Contains(Damage.Type.MISS)) return;
        
        bool isCovered = Coverable(
            context.attacker.GetHex(),
            hexPosition, 
            coverDirection
            );
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

    public Transform GetTsf => transform;

    public int GetHitRateModifier(Unit attacker)
    {
        return 100000;
    }

    public UnityEvent<int, int> OnHpChanged => _onHpChanged;

    #endregion

    // Get the direction from the player to the cover
    public Hex.Direction GetCoverDirections()
    {
        return coverDirection;
    }

    public static bool Coverable(Vector3Int atkHex, Vector3Int targetHex, Hex.Direction coverDirection)
    {
        Vector3 target = Hex.Hex2World(targetHex) + Vector3.up;
        Vector3 atk = Hex.Hex2World(atkHex) + Vector3.up;
        
        Vector3 targetToAtkRay = (atk - target).normalized;
        Vector3 midVector = Hex.Hex2World(Hex.GetDirectionHex(coverDirection));
        
        // is coverToAtkFrom between cwVector and ccwVector?
        float angle = Vector3.Angle(midVector, targetToAtkRay);
        if (angle <= 61)
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

    private void OnValidate()
    {
        // ���� ��ġ�� �������� Coverable�� ���� �� ������, �� Coverable�� ������ �޶�� �Ѵ�.
        if (CheckMultipleDirection())
        {
            Debug.LogError("���� ��ġ�� �������� Coverable�� ���� �� ������, �� Coverable�� ������ �޶�� �Ѵ�.");
        }
        
        // ������ Ȯ�� �ϰ�, �ش� ���⿡ ���߾ Rotation �缳��. rotation�� 0�ϰ�� ������ ������ �Ĵٺ�
        transform.rotation = Quaternion.Euler(0, (int) coverDirection * 60, 0);
    }

    private bool CheckMultipleDirection()
    {
        // check if there are multiple coverable objects in the same position
        CoverableObj[] coverables = FindObjectsOfType<CoverableObj>();
        if (coverables.Length <= 1) return false;
        
        // check if there are multiple coverable objects in the same position
        CoverableObj[] samePosCoverables = coverables.Where(c => c.hexPosition == hexPosition).ToArray();
        if (samePosCoverables.Length <= 1) return false;
        
        // check if there are multiple coverable objects in the same position
        CoverableObj[] sameDirCoverables = samePosCoverables.Where(c => c.coverDirection == coverDirection).ToArray();
        
        return sameDirCoverables.Length > 1;
    }

    public override void SetVisible(bool value)
    {
        //if editor mode, value always true
        if (GameManager.instance.CompareState(GameState.EDITOR)) return;
        
        _visible = value;
        // if vis
        
        // matrial�� light cover material�� �߰�. �� ���� material�� ������ �־�� ��.
        // light cover material�� shader�� transparent�̾�� ��.
        
        // if visible, light cover material�� �߰�
        // if not visible, light cover material�� ����
        
        if (value)
        {
            meshRenderer.materials = new[] {meshRenderer.material, LightCoverMaterial};
        }
        else
        {
            meshRenderer.materials = new[] {meshRenderer.material};
        }
        
    }

    public override bool IsVisible()
    {
        return _visible;
    }
}

public enum CoverType
{
    LIGHT,
    HEAVY,
    NONE
}