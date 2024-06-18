using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine
{
    public List<Bullet> bullets;
    public Magazine(int maxAmmo) 
    {
        bullets = new List<Bullet>();
        for (int i = 0; i < maxAmmo; i++) 
        {
            LoadBullet();
        }
    }

    public void LoadBullet()
    {
        bullets.Add(new Bullet());
    }
    public Bullet GetNextBullet() 
    {
        if (bullets.Count == 0) return new Bullet();
        return bullets[0];
    }
    public Bullet UseBullet() 
    {
        Bullet b = bullets[0];
        bullets.RemoveAt(0);
        return b;
    }
    public void SetGold(int index, BulletData data) 
    {
        if (bullets.Count < index) 
        {
            Debug.LogError("Magazine Size Error");
            return;
        }

        bullets[index].isGoldenBullet = true;
        bullets[index].data = data;
    }
    public void ClearEffectAll()
    {
        for (int i = 0; i < bullets.Count; i++) ClearEffect(i);
    }
    public void ClearEffect(int index)
    {
        bullets[index].isGoldenBullet = false;
        bullets[index].data = new();
    }
}
public class Bullet
{
    public BulletData data = new();
    public bool isGoldenBullet = false;
}
public class BulletData 
{
    public int range;
    public int damage;
    public int hitRate;
    public int criticalChance;
    public int criticalDamage;

    public BulletData()
    {
        range = 0;
        damage = 0;
        hitRate = 0;
        criticalChance = 0;
        criticalDamage = 0;
    }
}