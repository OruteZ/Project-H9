using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Barrel : TileObject, IDamageable
{
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    [SerializeField] private GameObject _firePrefab;

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
        if (args.Length != 2) throw new Exception("Invalid args length. Expected 2.");

        maxHp = int.Parse(args[0]);
        currentHp = int.Parse(args[1]);
    }
    protected override void SetTile(Tile t)
    {
        base.SetTile(t);
        t.walkable = false;
    }

    private void OnHit(Damage context)
    {    
        if (_onHitFlag is false) return;
        _onHitFlag = false;

        if (!context.Contains(Damage.Type.MISS)) return;

        Damage selfDamage = new(1, 1, Damage.Type.DEFAULT, context.attacker, null, this);
        TakeDamage(selfDamage);
    }
    #region IDamageable
    public void TakeDamage(Damage damage)
    {
        currentHp -= damage.GetFinalAmount();
        if (currentHp <= 0)
        {
            currentHp = 0;

            if (objectType == TileObjectType.TNT_BARREL) Explode();
            else if (objectType == TileObjectType.OIL_BARREL) CatchFire();

            tile.walkable = true;
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
        int distance = Hex.Distance(attacker.hexPosition, hexPosition);
        int range = attacker.weapon.GetRange();

        return (distance > range ? -100 : 100);
    }

    public UnityEvent<int, int> OnHpChanged => _onHpChanged;

    #endregion

    private bool _onHitFlag;
    public void SetFlag()
    {
        _onHitFlag = true;
    }

    const int EXPLOSION_RANGE_BREAK = 1;
    const int EXPLOSION_RANGE_50 = 1;
    const int EXPLOSION_RANGE_25 = 2;
    private void Explode()
    {
        List<IDamageable> targets = FieldSystem.GetAllDamageable();
        foreach (IDamageable target in targets)
        {
            int distance = Hex.Distance(hexPosition, target.GetHex());
            int damage = 0;

            if (distance == EXPLOSION_RANGE_50)
            {
                damage = Mathf.FloorToInt(target.GetMaxHp() * (50 / 100.0f));
            }
            else if (distance == EXPLOSION_RANGE_25)
            {
                damage = Mathf.FloorToInt(target.GetMaxHp() * (25 / 100.0f));
            }
            else continue;

            if (target is CoverableObj cObj && distance == EXPLOSION_RANGE_BREAK)
            {
                damage = cObj.GetMaxHp();
            }

            Damage context = new(damage, damage, Damage.Type.DEFAULT, null, this, target);
            target.TakeDamage(context);
        }

        //_visualEffect.Explode();
    }
    const int FIRE_RANGE = 2;
    private void CatchFire() 
    {
        IEnumerable<Tile> tiles = FieldSystem.tileSystem.GetTilesInRange(hexPosition, FIRE_RANGE);
        foreach (var tile in tiles)
        {
            GameObject fire = Instantiate(_firePrefab, tile.gameObject.transform.position, Quaternion.identity);
            fire.transform.SetParent(transform.parent);
            fire.GetComponent<FireFloor>().SetUp(tile.hexPosition, 20.0f, 30);
        }
    }
}
