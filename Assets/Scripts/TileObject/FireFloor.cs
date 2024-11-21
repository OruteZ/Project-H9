using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFloor : TileObject
{
    [SerializeField] private GameObject _fireFloorElementPrefab;
    private Dictionary<Tile, GameObject> _ffElements;
    private IEnumerable<Tile> _tiles;

    [SerializeField] private int fireRange;
    [SerializeField] private float percentDamage;
    [SerializeField] private int duration;
    private int durationCount = 0;
    public List<IDamageable> burnUnitInThisTurn { get; private set; }

    [SerializeField] private GameObject _fireEffectPrefab;
    [SerializeField] private List<GameObject> _fireEffects;

    public override string[] GetArgs()
    {
        return new string[] { percentDamage.ToString(), duration.ToString() };
    }
    public override void SetArgs(string[] args) { }
    public void SetUp(Vector3Int hexPosition, int fireRange, float percentDamage, int duration)
    {
        this.hexPosition = hexPosition;
        this.fireRange = fireRange;
        this.percentDamage = percentDamage;
        this.duration = duration;
        durationCount = duration;
        burnUnitInThisTurn = new();

        base.SetUp();

        if (tile == null)
        {
            Debug.LogError("타일이 없는 곳으로 Tile Object 배치 : hexPosition = " + hexPosition);
            throw new Exception();
        }

        _tiles = FieldSystem.tileSystem.GetTilesInRange(hexPosition, fireRange);
        _ffElements = new();

        foreach (var t in _tiles)
        {
            GameObject fire = Instantiate(_fireFloorElementPrefab, t.transform.position, Quaternion.identity);
            fire.transform.SetParent(transform.parent);
            fire.SetActive(false);
            _ffElements.Add(t, fire);
        }
    }
    public void Fire()
    {
        int sampleCount = 5;
        List<Vector3> pos = new();
        foreach (var e in _ffElements)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                float theta = UnityEngine.Random.Range(0, 360.0f) * 3.14f / 180;
                float p = UnityEngine.Random.Range(0, 1.0f);

                float x = e.Key.transform.position.x + p * radius * Mathf.Cos(theta);
                float z = e.Key.transform.position.z + p * radius * Mathf.Sin(theta);

                pos.Add(new Vector3(x, e.Key.transform.position.y, z));
            }
        }

        StartCoroutine(FireAnimation(pos));

        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnStart);
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) => CheckDamegeable());

        foreach (var e in _ffElements)
        {
            //if (e.Key.GetTileObjects<FireFloorElement>().Length > 0) continue;
            FieldSystem.tileSystem.AddTileObject(e.Value.GetComponent<FireFloorElement>());
            e.Value.SetActive(true);
            e.Value.GetComponent<FireFloorElement>().SetUp(e.Key.hexPosition, this);
        }

        CheckDamegeable();
    }

    private void OnTurnStart()
    {
        Unit turnOwner = FieldSystem.turnSystem.turnOwner;
        if (turnOwner is Player)
        {
            durationCount--;
            if (durationCount <= 0)
            {
                RemoveSelf();
                return;
            }
        }
        burnUnitInThisTurn.Remove(turnOwner);
        CheckDamegeable();
    }
    public void CheckDamegeable()
    {
        foreach (var f in _ffElements)
        {
            Unit unit = FieldSystem.unitSystem.GetUnit(f.Key.hexPosition);
            if (unit != null)
            {
                float dmg = percentDamage;
                FireFloorElement[] elements = f.Key.GetTileObjects<FireFloorElement>();
                foreach (var e in elements)
                {
                    if (e.gameObject == f.Value) continue;
                    if (e.fireFloorController.burnUnitInThisTurn.Contains(unit)) continue;

                    if (dmg < e.fireFloorController.percentDamage) 
                    {
                        dmg = e.fireFloorController.percentDamage; 
                    }
                }

                if (dmg == percentDamage)
                {
                    Burn(unit);
                }
            }

            TileObject[] tObj = f.Key.GetTileObjects<TileObject>();
            if (tObj.Length > 0 && tObj[0] is Barrel b)
            {
                Burn(b);
            }
        }

    }
    private void Burn(IDamageable target)
    {
        if (burnUnitInThisTurn.Contains(target)) return;
        int damage = Mathf.FloorToInt(target.GetMaxHp() * percentDamage / 100.0f);
        if (damage <= 0) damage = 1;
        Damage damageContext = new Damage
            (
            damage,
            damage,
            Damage.Type.BURNED,
            null,
            this,
            target
            );

        target.TakeDamage(damageContext);
        burnUnitInThisTurn.Add(target);
    }
    protected internal override void RemoveSelf()
    {
        FieldSystem.turnSystem.onTurnChanged.RemoveListener(OnTurnStart);
        FieldSystem.unitSystem.onAnyUnitMoved.RemoveListener((u) => CheckDamegeable());
        foreach (var e in _ffElements.Values)
        {
            e.GetComponent<FireFloorElement>().ForcedDestroy();
        }
        for (int i = _fireEffects.Count - 1; i >= 0; i--)
        {
            Destroy(_fireEffects[i]);
        }
        base.RemoveSelf();
    }

    private const float radius = 1.0f;
    IEnumerator FireAnimation(List<Vector3> pos)
    {

        float time = 0;
        while (pos.Count > 0)
        {
            for (int i = pos.Count - 1; i >= 0; i--)
            {
                if (time > Vector3.Distance(transform.position, pos[i]))
                {
                    Instantiate(_fireEffectPrefab, pos[i], Quaternion.identity);
                    pos.Remove(pos[i]);
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
            time += Time.deltaTime * 20;
        }
    }
}
