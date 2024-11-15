using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFloor : TileObject
{ 
    [SerializeField] private float percentDamage;
    [SerializeField] private int duration;
    private int durationCount = 0;
    private List<IDamageable> burnUnitInThisTurn;

    public override string[] GetArgs()
    {
        return new string[] { percentDamage.ToString(), duration.ToString() };
    }

    public override void SetArgs(string[] args) { }

    public void SetUp(Vector3Int hexPosition, float percentDamage, int duration)
    {
        this.hexPosition = hexPosition;
        this.percentDamage = percentDamage;
        this.duration = duration;
        durationCount = duration;
        burnUnitInThisTurn = new();

        base.SetUp();

        if (tile == null)
        {
            Debug.LogError("Ÿ���� ���� ������ Tile Object ��ġ : hexPosition = " + hexPosition);
            throw new Exception();
        }

        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) => CheckDamegeable());
    }

    private void OnTurnEnd() 
    {
        durationCount--;
        if (durationCount <= 0) 
        {
            RemoveSelf();
            return;
        }
        burnUnitInThisTurn.Clear();
        CheckDamegeable();
    }
    public void CheckDamegeable() 
    {
        Unit unit = FieldSystem.unitSystem.GetUnit(hexPosition);
        if (unit != null) 
        {
            Burn(unit);
        }

        List<TileObject> tObj = FieldSystem.tileSystem.GetTileObject(hexPosition);
        if (tObj.Count > 0 && tObj[0] is Barrel b)
        {
            Burn(b);
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

    public void ForcedDestroy()
    {
        RemoveSelf();
    }
}
