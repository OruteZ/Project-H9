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

    private const float FIRE_RADIUS = 1.0f;
    private const float FIRE_SPEED = 20;

    private bool isDestorying = false;

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
            GameObject fireTile = Instantiate(_fireFloorElementPrefab, t.transform.position, Quaternion.identity);
            fireTile.transform.SetParent(transform);
            fireTile.SetActive(false);
            _ffElements.Add(t, fireTile);
        }
    }
    public void Fire()
    {
        int sampleCount = 7;
        List<Vector3> pos = new();
        foreach (var e in _ffElements)
        {
            int randomSampleCnt = sampleCount + UnityEngine.Random.Range(0, 3);
            for (int i = 0; i < sampleCount; i++)
            {
                float theta = UnityEngine.Random.Range(0, 360.0f) * 3.14f / 180;
                float p = UnityEngine.Random.Range(0, 1.0f);

                float x = e.Key.transform.position.x + p * FIRE_RADIUS * Mathf.Cos(theta);
                float z = e.Key.transform.position.z + p * FIRE_RADIUS * Mathf.Sin(theta);

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
        if (isDestorying) return;
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
        if (isDestorying) return;
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

        if (target is Barrel)
        {
            StartCoroutine(DelayedExplode(target, damageContext));
        }
        else
        {
            target.TakeDamage(damageContext);
        }
        burnUnitInThisTurn.Add(target);
    }
    IEnumerator DelayedExplode(IDamageable target, Damage dmg) 
    {
        yield return new WaitForSeconds(0.5f);
        target.TakeDamage(dmg);
    }
    protected override void RemoveSelf()
    {
        isDestorying = true;
        StartCoroutine(FadingFireFloor());
    }
    IEnumerator FadingFireFloor() 
    {
        FieldSystem.turnSystem.onTurnChanged.RemoveListener(OnTurnStart);
        FieldSystem.unitSystem.onAnyUnitMoved.RemoveListener((u) => CheckDamegeable());
        float alpha = 1;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            foreach (var e in _ffElements.Values)
            {
                Color c = e.transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
                c.a = alpha;
                e.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = c;
            }
            for (int i = _fireEffects.Count - 1; i >= 0; i--)
            {
                var m = _fireEffects[i].GetComponent<ParticleSystem>().main;
                var c = m.startColor.color;
                c.a = alpha;
                m.startColor = c;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(_fireEffects[0].GetComponent<ParticleSystem>().main.startLifetime.constant);
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

    IEnumerator FireAnimation(List<Vector3> pos)
    {
        float time = 0;

        while (pos.Count > 0)
        {
            for (int i = pos.Count - 1; i >= 0; i--)
            {
                if (time > Vector3.Distance(transform.position, pos[i]))
                {
                    GameObject fe = Instantiate(_fireEffectPrefab, pos[i], Quaternion.identity);
                    fe.transform.localScale *= (1.5f - Vector3.Distance(transform.position, pos[i]) / (FIRE_RADIUS * 5));
                    fe.transform.localScale *= UnityEngine.Random.Range(0.8f, 1.2f);
                    fe.transform.SetParent(this.transform);
                    _fireEffects.Add(fe);

                    pos.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(Time.deltaTime);
            time += Time.deltaTime * FIRE_SPEED;
        }
    }
}
