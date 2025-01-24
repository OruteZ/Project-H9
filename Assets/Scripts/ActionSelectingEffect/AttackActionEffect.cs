using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackActionEffect : BaseSelectingActionEffect
{
    private Dictionary<Vector3Int, GameObject> _baseEffect;

    private Coroutine _dynamicCoroutine;
    
    private Canvas _effectCanvas;
    private HitRateUI _hitRateUI;

    protected override void OnSetup()
    {
        _baseEffect = new Dictionary<Vector3Int, GameObject>();

        _effectCanvas = effector.GetCanvas();
        _hitRateUI = Instantiate(setting.hitRateUIPrefab, _effectCanvas.transform).GetComponent<HitRateUI>();
        _hitRateUI.SetupEffect(_effectCanvas);
    }

    public override void StopEffect()
    {
        _baseEffect.Values.ToList().ForEach(Destroy);
        _baseEffect.Clear();
        
        effector.StopCoroutine(_dynamicCoroutine);
    }

    public override void ShowEffect(Unit user)
    {
        SetBaseInWeaponRange(user);
        SetBaseOnUnitTile(user);
        
        bool isRepeater = user.weapon.GetWeaponType() is ItemType.Repeater;
        bool hasSweetSpotSkill = user.GetDisplayableEffects().Any(e => e is SweetSpotEffect);
        if (isRepeater && hasSweetSpotSkill)
        {
            SetBaseInSweetSpot(user);
        }
        
        _dynamicCoroutine = effector.StartCoroutine(DynamicCoroutine(user));
    }
#region BASE EFFECT
    private void SetBaseInWeaponRange(Unit user)
    {
        int range = Mathf.Min(user.weapon.GetRange(), user.stat.sightRange);
        var tiles = FieldSystem.tileSystem.GetTilesInRange(user.hexPosition, range);
        var tilesInVisible = tiles.Where(
            tile => FieldSystem.tileSystem.VisionCheck(tile.hexPosition, user.hexPosition, true)
            ).ToList();

        foreach (Tile tile in tilesInVisible)
        {
            InstantiateTile(tile.hexPosition, setting.inWeaponRangeColor);
        }
    }
    
    private void SetBaseOnUnitTile(Unit user)
    {
        List<IDamageable> targets = FieldSystem.GetAllDamageable();
        foreach (IDamageable target in targets) if((Unit)target != user)
        {
            bool check = RayCheck(target) && VisionCheck(target) && SightCheck(target);
            if (check is false) continue;
            
            bool rangeCheck = WeaponRangeCheck(target);
            bool shotgunOutRangeFlag = user.weapon.GetWeaponType() is ItemType.Shotgun && !rangeCheck;

            if (rangeCheck) InstantiateTile(target.GetHex(), setting.damageableColor);
            if (shotgunOutRangeFlag) InstantiateTile(target.GetHex(), setting.shotgunOutRangeColor);
        }

        return;
        
        //================ Local Function ================
        bool WeaponRangeCheck(IDamageable target) => user.weapon.GetRange() >= 
                                                     Hex.Distance(user.hexPosition, target.GetHex());

        bool SightCheck(IDamageable target) => user.stat.sightRange >= 
                                               Hex.Distance(user.hexPosition, target.GetHex());

        bool VisionCheck(IDamageable target) => 
            FieldSystem.tileSystem.VisionCheck(user.hexPosition, target.GetHex(), true);

        bool RayCheck(IDamageable target) => 
            FieldSystem.tileSystem.RayThroughCheck(user.hexPosition, target.GetHex());
    }
    
    private void SetBaseInSweetSpot(Unit user)
    {
        int ssRange = user.weapon is Repeater repeater ? repeater.GetSweetSpot() : 0;
        IEnumerable<Tile> tiles = 
            FieldSystem.tileSystem.GetTilesOutLine(user.hexPosition, ssRange);

        foreach (Tile tile in tiles) if(tile.hexPosition != user.hexPosition)
        {
            // vision check
            if(FieldSystem.tileSystem.VisionCheck(user.hexPosition, tile.hexPosition, true) is false) continue;
            // ray check
            if(FieldSystem.tileSystem.RayThroughCheck(user.hexPosition, tile.hexPosition) is false) continue;
            // sight range check
            if(Hex.Distance(user.hexPosition, tile.hexPosition) > user.stat.sightRange) continue;
            
            if (FieldSystem.GetDamageable(tile.hexPosition) is not null)
            {
                InstantiateTile(tile.hexPosition, setting.sweetSpotOnTargetColor);
            }
            else
            {
                InstantiateTile(tile.hexPosition, setting.sweetSpotColor);
            }
        }
    }

    private void InstantiateTile(Vector3Int pos, Color color)
    {
        if (_baseEffect.TryGetValue(pos, out GameObject obj))
        {
            obj.GetComponent<Renderer>().material.color = color;
            return;
        }
        
        
        // get value from setting
        GameObject defaultTile = setting.tileEffectDefault;
        
        // instantiate
        Vector3 spawnPoint = Hex.Hex2World(pos);
        spawnPoint.y += 0.02f;
        GameObject instance = Instantiate(defaultTile, spawnPoint, Quaternion.identity);

        // Setting Color
        if (instance.TryGetComponent(out Renderer renderer) is false)
        {
            Material matCopy = new (renderer.material);
            matCopy.color = color;
            renderer.material = matCopy;
        }
        else
        {
            Debug.LogError("Default Tile GameObject dost not have Renderer Component");
        }
        
        _baseEffect.Add(pos, instance);
    }
#endregion

#region DYNAMIC EFFECT

    private IEnumerator DynamicCoroutine(Unit user)
    {
        while (true)
        {
            if (Player.TryGetMouseOverTilePos(out Vector3Int target) is false)
            {
                _hitRateUI.OffTarget();
                yield return null;
                continue;
            }

            IDamageable damageable = FieldSystem.GetDamageable(target);
            if (damageable is null || (Unit)damageable == user)
            {
                _hitRateUI.OffTarget();
                yield return null;
                continue;
            }
            
            // vision check
            if(FieldSystem.tileSystem.VisionCheck(user.hexPosition, target) is false)
            {
                _hitRateUI.OffTarget();
                yield return null;
                continue;
            }
            
            // ray check
            if(FieldSystem.tileSystem.RayThroughCheck(user.hexPosition, target) is false)
            {
                _hitRateUI.OffTarget();
                yield return null;
                continue;
            }
            
            // sight range check
            if(Hex.Distance(user.hexPosition, target) > user.stat.sightRange)
            {
                _hitRateUI.OffTarget();
                yield return null;
                continue;
            }

            float hitRate = user.weapon.GetFinalHitRate(damageable);

            bool hasGoldenBullet =
                user.weapon.magazine.GetNextBullet().isGoldenBullet &&
                user.GetAllPassiveList().Any(p => p.index == 21006);

            if (hasGoldenBullet) hitRate += 20;
            
            _hitRateUI.SetTarget(damageable, hitRate);
            
            
            yield return null;
        }
        
    }

#endregion
}