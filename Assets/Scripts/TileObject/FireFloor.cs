using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFloor : TileObject
{ 
    [SerializeField] private float percentDamage;
    [SerializeField] private int duration;
    private int durationCount = 0;
    private List<Unit> burnUnitInThisTurn;

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
            Debug.LogError("타일이 없는 곳으로 Tile Object 배치 : hexPosition = " + hexPosition);
            throw new Exception();
        }

        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) => CheckUnit());
        CheckUnit();
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
        CheckUnit();
    }
    private void CheckUnit() 
    {
        Unit unit = FieldSystem.unitSystem.GetUnit(hexPosition);
        if (unit is null) return;

        Burn(unit);
    }
    private void Burn(Unit unit)
    {
        if (burnUnitInThisTurn.Contains(unit)) return;
        int damage = Mathf.FloorToInt(unit.GetMaxHp() * percentDamage / 100.0f);
        Damage damageContext = new Damage
            (
            damage,
            damage,
            Damage.Type.BURNED,
            null,
            this,
            unit
            );

        unit.TakeDamage(damageContext);
        burnUnitInThisTurn.Add(unit);
    }
}
