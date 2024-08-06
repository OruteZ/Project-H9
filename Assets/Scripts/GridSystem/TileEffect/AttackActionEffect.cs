using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackActionEffect : BaseSelectingActionEffect
{
    private Dictionary<Vector3Int, GameObject> _baseEffect;
    private Dictionary<Vector3Int, GameObject> _dynamicEffect;

    private Coroutine _dynamicCoroutine;
    
    private RectTransform _effectCanvas;
    private RectTransform _aimEffectRectTsf;
    private RectTransform _aimEffect;
    
    public override void StopEffect()
    {
        _baseEffect.Values.ToList().ForEach(Destroy);
        _dynamicEffect.Values.ToList().ForEach(Destroy);
        
        _baseEffect.Clear();
        _dynamicEffect.Clear();
        effecter.StopCoroutine(_dynamicCoroutine);
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
        
        _dynamicCoroutine = effecter.StartCoroutine(DynamicCoroutine(user));
    }
#region BASE EFFECT
    private void SetBaseInWeaponRange(Unit user)
    {
        int range = Mathf.Min(user.weapon.GetRange(), user.stat.sightRange);
        var tiles = FieldSystem.tileSystem.GetTilesInRange(user.hexPosition, range);
        var tilesInVisible = tiles.Where(
            tile => FieldSystem.tileSystem.VisionCheck(tile.hexPosition, user.hexPosition)
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
            FieldSystem.tileSystem.VisionCheck(user.hexPosition, target.GetHex());

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
            if(FieldSystem.tileSystem.VisionCheck(user.hexPosition, tile.hexPosition) is false) continue;
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
            if (Player.TryGetMouseOverTilePos(out var target) is false)
            {
                _aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }

            IDamageable damageable = FieldSystem.GetDamageable(target);
            if (damageable is null || (Unit)damageable == user)
            {
                _aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            // vision check
            if(FieldSystem.tileSystem.VisionCheck(user.hexPosition, target) is false)
            {
                _aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            // ray check
            if(FieldSystem.tileSystem.RayThroughCheck(user.hexPosition, target) is false)
            {
                _aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            // sight range check
            if(Hex.Distance(user.hexPosition, target) > user.stat.sightRange)
            {
                _aimEffectRectTsf.gameObject.SetActive(false);
                yield return null;
                continue;
            }
            
            // set rect
            _aimEffectRectTsf.gameObject.SetActive(true);

            float hitRate = user.weapon.GetFinalHitRate(damageable);

            bool hasGoldenBullet =
                user.weapon.magazine.GetNextBullet().isGoldenBullet &&
                user.GetAllPassiveList().Any(p => p.index == 21006);

            if (hasGoldenBullet) hitRate += 20;

            Vector2 viewportPosition = 
                Camera.main.WorldToViewportPoint(
                    Hex.Hex2World(damageable.GetHex()) + Vector3.up * 
                    (0.5f));
            
            var sizeDelta = _effectCanvas.sizeDelta;
            
            Vector2 worldObjectScreenPosition = new Vector2(
                ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
                ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
            _aimEffectRectTsf.anchoredPosition = worldObjectScreenPosition;
            
            float size = hitRate * 0.01f;
            _aimEffect.localScale = new Vector3(size, size, 1);
            
            //hitrate text
            _aimEffectRectTsf.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                $"{hitRate}%";
            
            yield return null;
        }
        
    }

#endregion
}