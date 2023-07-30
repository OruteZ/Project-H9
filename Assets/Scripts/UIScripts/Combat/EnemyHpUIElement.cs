using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpUIElement : UIElement
{
    private Slider _hpBar;

    private Enemy _enemy;
    private int prevHp;

    // Start is called before the first frame update
    void Start()
    {
        _hpBar = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemy is not null)
        {

        }
    }

    public void SetEnemyHpUI(Enemy enemy)
    {
        _enemy = enemy;
        int maxHp = enemy.GetStat().maxHp;
        int curHp = enemy.GetStat().curHp;

        _hpBar.maxValue = maxHp;
        _hpBar.minValue = 0;
        _hpBar.value = curHp;

        prevHp = curHp;
    }
    public void ClearEnemyHpUI()
    {
        _enemy = null;
    }
}
