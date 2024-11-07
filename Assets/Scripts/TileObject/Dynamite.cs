using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DynamiteVisualEffect))]
public class Dynamite : TileObject
{
    public int durationTurn;
    public int radius;
    public int damage;
    public Unit owner;
    
    private DynamiteVisualEffect _visualEffect;
    
    private void Awake()
    {
        TryGetComponent(out _visualEffect);
    }
    
    public void SetUp(Unit owner, Vector3Int hexPosition, int durationTurn, int radius, int damage)
    {
        this.owner = owner;
        this.durationTurn = durationTurn;
        this.radius = radius;
        this.damage = damage;
        this.hexPosition = hexPosition;
        
        base.SetUp();
        
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
    }
    
    public override string[] GetArgs()
    {
        return new[]
        {
            owner.ToString(),
            durationTurn.ToString(),
            radius.ToString(),
            damage.ToString()
        };
    }

    public override void SetArgs(string[] args)
    {
        return;
    }
    
    public void OnTurnEnd()
    {
        durationTurn--;
        if (durationTurn <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        List<IDamageable> targets = FieldSystem.GetAllDamageable();
        foreach (IDamageable target in targets)
        {
            int distance = Hex.Distance(hexPosition, target.GetHex());
            if (distance > radius) continue;
            
            Damage context = new (damage, damage, Damage.Type.DEFAULT, owner, null, target);
            target.TakeDamage(context);
        }

        _visualEffect.Explode();
    }
}