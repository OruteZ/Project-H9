using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Pool;

public enum TileEffectType
{
    None = 0,
    Normal = 1,
    Friendly = 2,
    Hostile = 3,
    Impossible = 4,
    Invisible = 5,
    Ammunition = 10,
    Saloon = 11,
    Sheriff = 12,
}

/// <summary>
/// 타일을 넘겨받으면 원하는 이펙트를 타일에 적용시켜주는 클래스
/// </summary>
public class TileEffectManager : Generic.Singleton<TileEffectManager>
{
    public GameObject friendlyEffect;
    public GameObject hostileEffect;
    public GameObject normalEffect;
    public GameObject impossibleEffect;
    public GameObject invisibleEffect;

    public GameObject ammunitionEffect;
    public GameObject salonEffect;
    public GameObject sheriffEffect;

    private Player _player;
    private Dictionary<Vector3Int, GameObject> _effectsBase;
    private Dictionary<Vector3Int, GameObject> _effectsRelatedTarget;

    private Coroutine _curCoroutine;

    public Material combatFowMaterial;

    [field: Header("Attack Effect")] 
    public GameObject attackTileEffect;
    public GameObject attackOutOfRangeEffect;
    public GameObject attackUnitEffect;
    public GameObject attackSweetSpotEffect;
    public GameObject attackSweetSpotUnitEffect;
    public RectTransform aimEffectRectTsf;
    
    public RectTransform combatCanvas;
    public RectTransform aimEffect;

    [field: Header("Cover Effect")] 
    public GameObject coverMainEffect;
    public GameObject coverSubEffect;
    public Material coverObjMaterial;
    private List<CoverableObj> _coverableObjs;
    private Dictionary<Vector3Int, GameObject> _coverEffects;

    [field: Header("Barrel Effect")]
    public GameObject barrelNearEffect;
    public GameObject barrelFarEffect;
    private Dictionary<Vector3Int, GameObject> _barrelEffects;


    public void SetPlayer(Player p)
    {
        _player = p;
        p.onSelectedChanged.AddListener(TileEffectSet);
        p.onBusyChanged.AddListener(() =>
        {
            if (_player.IsBusy())
            {
                ClearEffect();
                
            }
            else TileEffectSet();
        });
    }

    private List<CustomOutline> _outlines = new();
    GameObject prevMouseOverObj = null;
    public void SetCoverEffect(GameObject coverObj)
    {
        ClearEffect(_coverEffects);
        ClearEffect(_barrelEffects);
        if (coverObj == prevMouseOverObj) return;

        if (_player.GetSelectedAction().GetActionType() != ActionType.Idle) return;
        if (_player != FieldSystem.turnSystem.turnOwner) return;
        if (coverObj != null)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(coverObj.GetComponent<TileObject>().hexPosition);
            CoverableObj[] coverObjects = tile.GetTileObjects<CoverableObj>();
            CoverTileEffect(coverObj, coverMainEffect);
            foreach (var obj in coverObjects)
            {
                if (obj.gameObject == coverObj) continue;
                CoverTileEffect(obj.gameObject, coverSubEffect);
            }
        }
        prevMouseOverObj = coverObj;
    }
    public void SetCoverableOutline(GameObject coverObj)
    {
        ClearOutlines();
        if (_player.GetSelectedAction().GetActionType() != ActionType.Idle) return;
        if (_player != FieldSystem.turnSystem.turnOwner) return;
        if (coverObj != null)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(coverObj.GetComponent<TileObject>().hexPosition);
            CoverableObj[] coverObjects = tile.GetTileObjects<CoverableObj>();
            SetTileObjectOutline(coverObj, new Color32(255, 217, 102, 255));
            foreach (var obj in coverObjects)
            {
                if (obj.gameObject == coverObj) continue;
                SetTileObjectOutline(obj.gameObject, new Color32(255, 242, 204, 128));
            }

            if (prevMouseOverObj != null && prevMouseOverObj != coverObj) SetCoverEffect(coverObj);
        }
    }
    public void SetBarrelEffect(GameObject barrel)
    {
        ClearEffect(_coverEffects);
        ClearEffect(_barrelEffects);
        if (barrel == prevMouseOverObj) return;

        if (_player.GetSelectedAction().GetActionType() != ActionType.Idle) return;
        if (_player != FieldSystem.turnSystem.turnOwner) return;
        if (barrel != null)
        {
            Tile tile = FieldSystem.tileSystem.GetTile(barrel.GetComponent<TileObject>().hexPosition);
            BarrelTileEffect(barrel);
        }
        prevMouseOverObj = barrel;
    }
    public void SetTileObjectOutline(GameObject tileObject)
    {
        ClearOutlines();
        if (_player.GetSelectedAction().GetActionType() != ActionType.Idle) return;
        if (_player != FieldSystem.turnSystem.turnOwner) return;
        if (tileObject != null)
        {
            SetTileObjectOutline(tileObject, new Color32(255, 0, 0, 255));
        }
    }
    #region PRIVATE

    private void TileEffectSet()
    {
        ClearEffect();
        switch (_player.GetSelectedAction().GetActionType())
        {
            case ActionType.Move:
                MovableTileEffect();
                break;
            case ActionType.Spin:
                break;
            case ActionType.Attack:
                AttackTileEffect();
                break;
            case ActionType.Dynamite:
                DynamiteTileEffect();
                break;
            case ActionType.Idle:
                ClearEffect(_effectsBase);
                ClearEffect(_effectsRelatedTarget);
                ClearEffect(_coverEffects);
                ClearEffect(_barrelEffects);
                break;
            case ActionType.Reload:
                break;
            case ActionType.Fanning:
                AttackTileEffect();
                break;
            case ActionType.None:
                break;
            case ActionType.Hemostasis:
                break;
            case ActionType.ItemUsing:
                ItemTileEffect();
                break;
            // case ActionType.Cover:
            //     CoverEffect();
            //     break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    

    #region MOVE

    [field: Header("Other Values")]
    [SerializeField] private float movableDelay;
    private float _movableEffectDelay;
    
    private void MovableTileEffect()
    {
        
        //if world state, get sight range, else get action po
        int range = 0;
        if(GameManager.instance.CompareState(GameState.COMBAT))
            range = _player.stat.curActionPoint / _player.GetAction<MoveAction>().GetCost();
        else
            range = _player.stat.sightRange;
        
        Vector3Int start = _player.hexPosition;

        var tiles = FieldSystem.tileSystem.GetWalkableTiles(start, range);
        foreach (Tile tile in tiles)
        {
            if (tile is null)
            {
                Debug.LogError("Tile is null");
                continue;
            }
            
            TileEffectType effectType = TileEffectType.Normal;
            if (GameManager.instance.CompareState(GameState.COMBAT))
            {
                int distance = Hex.Distance(tile.hexPosition, _player.hexPosition);
                int sightRange = _player.stat.sightRange;
                if (distance > sightRange) continue;
                if (!FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition)) continue;
                SetEffectBase(tile.hexPosition, effectType);
            }
            else //GameState.World
            {
                bool containsFog = tile.tileObjects.OfType<FogOfWar>().Any();
                if (containsFog) continue;
            }
        }

        _movableEffectDelay = movableDelay;
        _curCoroutine = StartCoroutine(MovableTileEffectCoroutine());
    }
    
    private IEnumerator MovableTileEffectCoroutine()
    {
        
        int range = 0;
        if(GameManager.instance.CompareState(GameState.COMBAT))
            range = _player.stat.curActionPoint / _player.GetAction<MoveAction>().GetCost();
        else
            range = int.MaxValue;
        
        int prevRouteLength = -1;
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            // check movable
            if (Player.TryGetMouseOverTilePos(out Vector3Int target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.unitSystem.GetUnit(target) is not null)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if(GameManager.instance.CompareState(GameState.WORLD) && GameManager.instance.IsPioneeredWorldTile(target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            // set route
            var route = FieldSystem.tileSystem.FindPath(_player.hexPosition, target);
            if (route is null)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            // is over movable range
            // route contains start position, so Count - 1
            if(route.Count - 1 <= range)
            {
                foreach (Vector3Int pos in route.Select(tile => tile.hexPosition))
                {
                    TileEffectType effectType = TileEffectType.Friendly;
                    Tile tile = FieldSystem.tileSystem.GetTile(pos);
                    foreach (Town tileObject in tile.tileObjects.OfType<Town>())
                    {
                        effectType = (tileObject).GetTileEffectType();
                    }
                    SetEffectTarget(pos, effectType);
                }
            }
            else
            {
                ClearEffect(_effectsRelatedTarget);
            }

            // check routePath
            if (prevRouteLength != route.Count)
            {
                UIManager.instance.gameSystemUI.playerInfoUI.summaryStatusUI.expectedApUsage = route.Count - 1;
                UIManager.instance.onPlayerStatChanged.Invoke();
            }
            prevRouteLength = route.Count;

        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region SHOOT
    private void AttackTileEffect()
    {
        IEnumerable<Tile> tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, _player.weapon.GetRange()).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));
        IEnumerable<Unit> units = FieldSystem.unitSystem.units.Where(
            unit => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) && unit is not Player);
        IEnumerable<TileObject> tObj = FieldSystem.tileSystem.GetAllTileObjects().Where(
            obj => obj is not CoverableObj && FieldSystem.tileSystem.VisionCheck(_player.hexPosition, obj.hexPosition));

        //white range tile
        foreach (var tile in tiles)
        {
            // condition 1 : no walkable tile
            if (!tile.walkable) continue;

            //// condition 1 : no unit on tile
            //if (FieldSystem.unitSystem.GetUnit(tile.hexPosition) is not null) continue;

            // condition 2 : in sight range
            if (Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            
            SetEffectBase(tile.hexPosition, attackTileEffect);
        }

        //target range tile
        List<Vector3Int> unitPositions = new();
        List<Vector3Int> tObjPositions = new();
        foreach (Unit u in units) unitPositions.Add(u.hexPosition);
        foreach (TileObject t in tObj) tObjPositions.Add(t.hexPosition);
        AttackTargetTileEffect(unitPositions);
        AttackTargetTileEffect(tObjPositions);

        //if weapon type is repeater && has sweet spot buff, show sweet spot
        if (_player.weapon is Repeater repeater && 
            _player.GetDisplayableEffects().Any(e => e is SweetSpotEffect))
        {
            var sweetSpot = (repeater).GetSweetSpot();
            if (sweetSpot < _player.stat.GetStat(StatType.SightRange))
            {
                //remove tiles in sweet spot range circle
                foreach (var pos in FieldSystem.tileSystem.GetTilesOutLine(_player.hexPosition, sweetSpot))
                {
                    if (_effectsBase.ContainsKey(pos.hexPosition))
                    {
                        Destroy(_effectsBase[pos.hexPosition]);
                        _effectsBase.Remove(pos.hexPosition);
                    }
                
                    SetEffectBase(pos.hexPosition, attackSweetSpotEffect);
                }
            
                //remove units in sweet spot range circle
                foreach (Unit unit in FieldSystem.unitSystem.units)
                {
                    if (unit is Player) continue;
                    if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, unit.hexPosition) is false) continue;
                    if (Hex.Distance(_player.hexPosition, unit.hexPosition) > _player.stat.sightRange) continue;
                    if (sweetSpot == Hex.Distance(_player.hexPosition, unit.hexPosition))
                    {
                        if (_effectsBase.ContainsKey(unit.hexPosition))
                        {
                            Destroy(_effectsBase[unit.hexPosition]);
                            _effectsBase.Remove(unit.hexPosition);
                        }
                    
                        SetEffectBase(unit.hexPosition, attackSweetSpotUnitEffect);
                    }
                }
            }
        }

        _curCoroutine = StartCoroutine(AttackTargetEffectCoroutine());
    }
    private void AttackTargetTileEffect(List<Vector3Int> targetHexPos)
    {
        for (int i = 0; i < targetHexPos.Count; i++) 
        {
            // condition 1 : raycheck true
            // condition 2 : vision check true
            // condition 3 : in sight range

            if (FieldSystem.tileSystem.RayThroughCheck(_player.hexPosition, targetHexPos[i]) is false) continue;
            if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, targetHexPos[i]) is false) continue;
            if (Hex.Distance(_player.hexPosition, targetHexPos[i]) > _player.stat.sightRange) continue;

            if (_effectsBase.ContainsKey(targetHexPos[i]))
            {
                Destroy(_effectsBase[targetHexPos[i]]);
                _effectsBase.Remove(targetHexPos[i]);
            }

            if (_player.weapon.GetRange() >= Hex.Distance(_player.hexPosition, targetHexPos[i]))
            {
                SetEffectBase(targetHexPos[i], attackUnitEffect);
            }
            else
            {
                if (_player.weapon.GetWeaponType() is not ItemType.Shotgun)
                    SetEffectBase(targetHexPos[i], attackOutOfRangeEffect);
            }
        }
    }

    private IEnumerator AttackTargetEffectCoroutine()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        while (true)
        {
            if (Player.TryGetMouseOverTilePos(out var targetHexPos) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            var targetUnit = FieldSystem.unitSystem.GetUnit(targetHexPos);
            var targetTObj = FieldSystem.tileSystem.GetAllTileObjects().Where(
            obj => obj.hexPosition == targetHexPos && obj is IDamageable && obj is not CoverableObj);

            //if ((targetUnit is null or Player) && targetTObj is null)
            //{
            //    aimEffectRectTsf.gameObject.SetActive(false);
            //    yield return null;
            //    continue;
            //}


            IDamageable targetObj = null;
            Transform targetTransform = null;
            if (targetUnit is not null && targetUnit is not Player)
            {
                targetObj = (IDamageable)targetUnit;
                targetTransform = targetUnit.transform;
            }
            else if (targetTObj is not null && targetTObj.Count() == 1 && player.GetSelectedAction() is AttackAction)
            {
                targetObj = (IDamageable)targetTObj.First();
                targetTransform = targetTObj.First().transform;
            }
            else
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            if (FieldSystem.tileSystem.VisionCheck(_player.hexPosition, targetHexPos) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            if (FieldSystem.tileSystem.RayThroughCheck(_player.hexPosition, targetHexPos) is false)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            if (Hex.Distance(_player.hexPosition, targetHexPos) > _player.weapon.GetRange()
                && _player.weapon is Shotgun)
            {
                aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            
            aimEffectRectTsf.gameObject.SetActive(true);
            var hitRate = _player.weapon.GetFinalHitRate(targetObj);
            float offset = 0;
            if (_player.GetSelectedAction() is FanningAction f)
            {
                offset += f.GetHitRateModifier();
            }
            else
            {
                if (_player.weapon.magazine.GetNextBullet().isGoldenBullet)
                {
                    foreach (var passive in _player.GetAllPassiveList())
                    {
                        if (passive.index == 21006)
                        {
                            offset += 20;
                            break;
                        }
                    }
                }
            }
            hitRate += offset;
            if (hitRate >= 100) hitRate = 100;
            if (hitRate <= 0) hitRate = 0;

            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(Hex.Hex2World(targetHexPos) + 
                                                                        Vector3.up * (targetTransform.localScale.y * 0.5f));
            var sizeDelta = combatCanvas.sizeDelta;
            
            Vector2 worldObjectScreenPosition = new Vector2(
                ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
                ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
            aimEffectRectTsf.anchoredPosition = worldObjectScreenPosition;
            
            float size = hitRate * 0.01f;
            aimEffect.localScale = new Vector3(size, size, 1);
            
            //hitrate text
            aimEffectRectTsf.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                $"{hitRate}%";
            

            // aimEffectRectTsf.gameObject.SetActive(false);
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region DYNAMITE
    private void DynamiteTileEffect()
    {
        int range = _player.GetAction<DynamiteAction>().GetThrowingRange();
        
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, range).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));

        foreach (var tile in tiles)
        {
            if(Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            
            var go =Instantiate(attackTileEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            _effectsBase.Add(tile.hexPosition, go);
        }

        _curCoroutine = StartCoroutine(DynamiteTargetEffectCoroutine());
    }

    private IEnumerator DynamiteTargetEffectCoroutine()
    {
        int expRange = _player.GetAction<DynamiteAction>().GetExplosionRange();
        int thrRange = _player.GetAction<DynamiteAction>().GetThrowingRange();
        
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            if (Hex.Distance(target, _player.hexPosition) > thrRange)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            var tiles = FieldSystem.tileSystem.GetTilesInRange(target, expRange);
            foreach (var pos in tiles.Select(tile => tile.hexPosition)) 
            {
                if(Hex.Distance(_player.hexPosition, pos) > _player.stat.sightRange) continue;
                SetEffectTarget(pos, TileEffectType.Friendly);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    
    #region USING ITEM
    
    private void ItemTileEffect()
    {
        int range = _player.GetAction<ItemUsingAction>().GetItem().GetData().itemRange;
        
        var tiles = FieldSystem.tileSystem.GetTilesInRange(_player.hexPosition, range).Where(
            tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));

        foreach (var tile in tiles)
        {
            if(Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            var go =Instantiate(attackTileEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            _effectsBase.Add(tile.hexPosition, go);
        }

        _curCoroutine = StartCoroutine(ItemTargetEffectCoroutine());
    }

    private IEnumerator ItemTargetEffectCoroutine()
    {
        int expRange = 0;
        int thrRange = _player.GetAction<ItemUsingAction>().GetItem().GetData().itemRange;
        
        while (true)
        {
            yield return null;
            ClearEffect(_effectsRelatedTarget);

            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }
            if (FieldSystem.tileSystem.GetTile(target).visible is false)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            if (Hex.Distance(target, _player.hexPosition) > thrRange)
            {
                ClearEffect(_effectsRelatedTarget);
                continue;
            }

            var tiles = FieldSystem.tileSystem.GetTilesInRange(target, expRange);
            foreach (var pos in tiles.Select(tile => tile.hexPosition)) 
            {
                if(Hex.Distance(_player.hexPosition, pos) > _player.stat.sightRange) continue;
                SetEffectTarget(pos, TileEffectType.Friendly);
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion

    #region COVER

    [Tooltip("The range of the cover effect")]
    [SerializeField] private int coverEffectRange;
    const float DIFF = 31;
    private void CoverTileEffect(GameObject coverObj, GameObject EffectObj)
    {
        Tile tile = FieldSystem.tileSystem.GetTile(coverObj.GetComponent<TileObject>().hexPosition);

        Hex.Direction dir = coverObj.GetComponent<CoverableObj>().GetCoverDirections();
        float ang = 360.0f -(int)dir * 60.0f;
        Vector2 coverDir = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));

        var coverTiles = FieldSystem.tileSystem.GetTilesInRange(tile.hexPosition, coverEffectRange);
        foreach (Vector3Int pos in coverTiles.Select(t => t.hexPosition))
        {
            if (pos == tile.hexPosition) continue;
            Vector2 posDir = Hex.Hex2Orth(pos) - Hex.Hex2Orth(tile.hexPosition);
            float angle = Vector2.SignedAngle(coverDir.normalized, posDir.normalized);
            if (angle < DIFF && angle > -DIFF)
            {
                //Debug.LogError(angle + " degree/ " + tile.hexPosition + " to "+pos);
                SetEffectTarget(pos, EffectObj, _coverEffects);
            }
        }
    }
    private void SetTileObjectOutline(GameObject tileObj, Color color)
    {
        tileObj.TryGetComponent(out CustomOutline outline);
        if (outline is null)
        {
            outline = tileObj.gameObject.AddComponent<CustomOutline>();
            _outlines.Add(outline);
        }
        outline.OutlineColor = color;
        outline.OutlineMode = CustomOutline.Mode.OutlineAll;
        outline.SetOutline();
    }
    private void ClearOutlines()
    {
        for (int i = _outlines.Count() - 1; i >= 0; i--)
        {
            if (_outlines[i] == null || _outlines[i].gameObject == null)
            {
                _outlines.Remove(_outlines[i]);
                continue;
            }
            _outlines[i].ClearOutline();
        }
    }
    #endregion

    #region BARREL
    [SerializeField] private RuntimeAnimatorController _fadeInOutAnimator;
    private Animator _animator;
    public float _tileEffectAlpha;
    private void BarrelTileEffect(GameObject barrelObj)
    {
        if (!barrelObj.TryGetComponent<Barrel>(out var barrel)) return;

        var tiles = FieldSystem.tileSystem.GetTilesInRange(barrel.hexPosition, Barrel.EXPLOSION_RANGE_25);
            //.Where(tile => FieldSystem.tileSystem.VisionCheck(_player.hexPosition, tile.hexPosition));

        foreach (var tile in tiles)
        {
            //if (Hex.Distance(_player.hexPosition, tile.hexPosition) > _player.stat.sightRange) continue;
            GameObject effectObj = null;

            int distance = Hex.Distance(barrel.hexPosition, tile.hexPosition);
            if (barrel.objectType == TileObjectType.TNT_BARREL && distance <= Barrel.EXPLOSION_RANGE_50)
            {
                effectObj = Instantiate(barrelNearEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            }
            else if (distance <= Barrel.EXPLOSION_RANGE_25) 
            {
                effectObj = Instantiate(barrelFarEffect, Hex.Hex2World(tile.hexPosition), Quaternion.identity);
            }
            _barrelEffects.Add(tile.hexPosition, effectObj);
        }
        _curCoroutine = StartCoroutine(BarrelEffectCoroutine());
    }

    private IEnumerator BarrelEffectCoroutine()
    {
        _animator.runtimeAnimatorController = _fadeInOutAnimator;
        _animator.enabled = true;
        _animator.Rebind();
        _animator.Play("Fade In & Out Tile Effect");
        while (true)
        {
            yield return null;
            foreach (var eo in _barrelEffects.Values) 
            {
                Color color = eo.GetComponent<Renderer>().material.color;
                color.a = _tileEffectAlpha;
                eo.GetComponent<Renderer>().material.color = color;
            }
        }
    }
    #endregion
    private new void Awake()
    {
        _animator = GetComponent<Animator>();

        base.Awake();

        _effectsBase = new Dictionary<Vector3Int, GameObject>();
        _effectsRelatedTarget = new Dictionary<Vector3Int, GameObject>();
        _coverEffects = new Dictionary<Vector3Int, GameObject>();
        _barrelEffects = new Dictionary<Vector3Int, GameObject>();

        _coverableObjs = new List<CoverableObj>();

    }

    private void ClearEffect()
    {
        // <Legacy>
        // var tiles = MainSystem.instance.tileSystem.GetAllTiles();
        // foreach (var tile in tiles) tile.effect = null;
        if(_curCoroutine is not null) StopCoroutine(_curCoroutine);
        
        ClearEffect(_effectsBase);
        ClearEffect(_effectsRelatedTarget);
        ClearEffect(_coverEffects);
        ClearEffect(_barrelEffects);

        _animator.enabled = false;

        aimEffectRectTsf.gameObject.SetActive(false);
        
        //remove cover effect
        foreach (CoverableObj coverable in _coverableObjs.Where(coverable => coverable != null))
        {
            var meshRenderer = coverable.meshRenderer;
            
            List<Material> materialList = meshRenderer.materials.ToList();
            
            if(materialList.Count <= 1) continue;
            materialList.RemoveAt(1);
            
            coverable.meshRenderer.materials = materialList.ToArray();
        }
    }

    private void SetEffectBase(Vector3Int position, TileEffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.02f;
        
        var gObject = Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
        _effectsBase.Add(position, gObject);
    }
    
    private void SetEffectBase(Vector3Int position, GameObject effect)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.02f;
        
        var gObject = Instantiate(effect, worldPosition, Quaternion.identity);
        _effectsBase.Add(position, gObject);
    }
    
    private void SetEffectTarget(Vector3Int position, TileEffectType type)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.03f;

        if (_effectsRelatedTarget.ContainsKey(position) is false)
        {
            var gObject = Instantiate(GetEffect(type), worldPosition, Quaternion.identity);
            _effectsRelatedTarget.Add(position, gObject);
        }
    }
    
    private void SetEffectTarget(Vector3Int position, GameObject effect, Dictionary<Vector3Int, GameObject> effects)
    {
        Vector3 worldPosition = Hex.Hex2World(position);
        worldPosition.y += 0.03f;

        if (effects.ContainsKey(position) is false)
        {
            var gObject = Instantiate(effect, worldPosition, Quaternion.identity);
            effects.Add(position, gObject);
        }
    }
    
    private GameObject GetEffect(TileEffectType type)
    {
        return type switch
        {
            TileEffectType.Friendly => friendlyEffect,
            TileEffectType.Hostile => hostileEffect,
            TileEffectType.Normal => normalEffect,
            TileEffectType.Impossible => impossibleEffect,
            TileEffectType.Invisible => invisibleEffect,
            TileEffectType.Ammunition => ammunitionEffect,
            TileEffectType.Saloon => salonEffect,
            TileEffectType.Sheriff => sheriffEffect,
            _ => null
        };
    }
    
    private static void ClearEffect(Dictionary<Vector3Int, GameObject> pool)
    {
        foreach (GameObject go in pool.Values)
        {
            Destroy(go);
        }
        
        pool.Clear();
    }
    #endregion
}
