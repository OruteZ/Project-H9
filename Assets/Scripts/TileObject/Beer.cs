using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : TileObject
{
    [SerializeField] private float healAmount;

    public override string[] GetArgs()
    {
        return new string[] { healAmount.ToString() };
    }
    public override void SetArgs(string[] args)
    {
        if (args.Length != 1) throw new Exception("Invalid args length. Expected 1.");

        healAmount = int.Parse(args[0]);
    }
    public override void SetUp()
    {
        base.SetUp();

        if (tile == null)
        {
            Debug.LogError("타일이 없는 곳으로 Tile Object 배치 : hexPosition = " + hexPosition);
            throw new Exception();
        }
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) => CheckUnit());
        CheckUnit();
    }
    private void CheckUnit()
    {
        Unit unit = FieldSystem.unitSystem.GetUnit(hexPosition);
        if (unit == null) return;

        Heal(unit);
        RemoveSelf();
    }
    private void Heal(Unit unit)
    {
        unit.stat.Recover(StatType.CurHp, Mathf.FloorToInt(unit.GetMaxHp() * healAmount / 100.0f), out var appliedValue);

        UIManager.instance.onHealed.Invoke(unit, appliedValue, Damage.Type.HEAL);
    }
}
