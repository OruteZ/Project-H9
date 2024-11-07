using System;
using System.Collections;
using System.Collections.Generic;
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

    private Material _material;
    private Color _originColor;
    private float _targetAlpha;

    public override void SetUp()
    {
        base.SetUp();
        _onHpChanged.RemoveAllListeners();
        
        currentHp = maxHp;

        _material = transform.GetChild(1).GetComponent<MeshRenderer>().material;
        _originColor = _material.color;
        _targetAlpha = 1;
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
        newUnit.AddCoverable(this);
    }

    private void OnHit(Damage context)
    {
    //     Debug.Log("Coverable On Hit : " + gameObject.name);
    //     
    //     CoverableObj[] otherCovers = tile.GetTileObjects<CoverableObj>();
    //     foreach (CoverableObj cover in otherCovers)
    //     {
    //         if (cover == this) continue;
    //
    //         if (cover.IsCovered(context.id)) return;
    //     }
    //     
    //     
    // set gizmo
    //     
    //     
    //     
    //     bool isCovered = Coverable(
    //         context.attacker.GetHex(),
    //         hexPosition, 
    //         coverDirection
    //     );
    //     if (!isCovered) return;
    //     
    //
    //     coveredDamages.Add(context.id);
    //     Debug.Log("Covered Damage : " + context.id + " : " + gameObject.name);
    //     
    if (_onHitFlag is false) return;
    _onHitFlag = false;
        
        if (!context.Contains(Damage.Type.MISS)) return;
        
        Damage selfDamage = new (1, 1, Damage.Type.DEFAULT, context.attacker, null, this);
        TakeDamage(selfDamage);
    }

    private void OnUnitMoved(Unit u)
    {
        u.onHit.RemoveListener(OnHit);
        u.onMoved.RemoveListener(OnUnitMoved);
        u.RemoveCoverable(this);
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

    public static bool CanCover(Vector3Int atkHex, Vector3Int targetHex, Hex.Direction coverDirection)
    {
        Vector3 target = Hex.Hex2World(targetHex);
        Vector3 atk = Hex.Hex2World(atkHex);
        
        Vector3 targetToAtkRay = (atk - target).normalized;
        Vector3 midVector = Hex.Hex2World(Hex.GetDirectionHex(coverDirection));
        
        // is coverToAtkFrom between cwVector and ccwVector?
        float angle = Vector3.Angle(midVector, targetToAtkRay);
        if (angle <= 31)
        {
            return true;
        }
            
        return false;
    }

    private bool gizmoFlag = false;
    
    [Space(10)]
    [SerializeField] private Vector3Int g_targetHex;
    [SerializeField]private Vector3Int g_atkFromHex;

    public void OnDrawGizmosSelected()
    {
        // if (!gizmoFlag) return;
        
        // show 2 vectors
        Vector3 target = Hex.Hex2World(g_targetHex) + Vector3.up;
        Vector3 atkFromPos = Hex.Hex2World(g_atkFromHex) + Vector3.up;
        
        Vector3 midVector = (Hex.Hex2World(Hex.GetDirectionHex(GetCoverDirections()))).normalized;
        Vector3 cwVector = Quaternion.AngleAxis(30, Vector3.up) * midVector;
        Vector3 ccwVector = Quaternion.AngleAxis(-30, Vector3.up) * midVector;
        Vector3 coverToAtkFrom = (atkFromPos - target).normalized;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target, target + midVector * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(target, target + cwVector * 10);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(target, target + ccwVector * 10);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(target, target + coverToAtkFrom * 10);
    }

    private void OnValidate()
    {
        // 같은 위치에 여러개의 Coverable이 있을 수 있으나, 각 Coverable의 방향은 달라야 한다.
        if (CheckMultipleDirection())
        {
            Debug.LogError("같은 위치에 여러개의 Coverable이 있을 수 있으나, 각 Coverable의 방향은 달라야 한다.");
        }
        
        // 방향을 확인 하고, 해당 방향에 맞추어서 Rotation 재설정. rotation이 0일경우 오른쪽 정면을 쳐다봄
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
        
        // matrial에 light cover material을 추가. 두 개의 material을 가지고 있어야 함.
        // light cover material은 shader가 transparent이어야 함.
        
        // if visible, light cover material을 추가
        // if not visible, light cover material을 제거
        
        meshRenderer.materials = value ? new[] {meshRenderer.material, LightCoverMaterial} : new[] {meshRenderer.material};
        
    }

    public override bool IsVisible()
    {
        return _visible;
    }

    private bool _onHitFlag;
    public void SetFlag()
    {
        _onHitFlag = true;
    }

    #region transparent
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Player>(out var c)) return;
        StopCoroutine(BecomeTransparent());
        StartCoroutine(BecomeTransparent());
    }
    IEnumerator BecomeTransparent()
    {
        _material.SetFloat("_Mode", 3);
        _targetAlpha = -1.0f;
        while (_material.color.a > _targetAlpha)
        {
            ChangeMaterialAlpha();
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(0.125f);
        _targetAlpha = 1.0f;
        while (_material.color.a < _targetAlpha)
        {
            ChangeMaterialAlpha();
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _material.SetFloat("_Mode", 1);

        yield break;
    }
    const int TRANS_SPEED = 20;
    private void ChangeMaterialAlpha()
    {
        Color c = _material.color;
        LerpCalculation.CalculateLerpValue(ref c.a, _targetAlpha, TRANS_SPEED);
        _material.color = c;
    }
    #endregion
}

public enum CoverType
{
    LIGHT,
    HEAVY,
    NONE
}