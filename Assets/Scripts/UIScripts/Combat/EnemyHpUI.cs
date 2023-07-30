using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpUI : UISystem
{
    [SerializeField] private GameObject _hpBarPrefab;
    [SerializeField] private GameObject _hpBars;

    private List<Enemy> _enemies = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetEnemyHpBars()
    {
        List<Unit> units = FieldSystem.unitSystem.units;
        _enemies = new List<Enemy>();
        foreach (Unit unit in units)
        {
            Debug.Log("type:" + unit.GetType());
            if (unit is Enemy)
            {
                _enemies.Add((Enemy)unit);
            }
        }

        Debug.Log("���� ��:" + _enemies.Count);

        for (int i = 0; i < _hpBars.transform.childCount; i++)
        {
            _hpBars.transform.GetChild(i).GetComponent<EnemyHpUIElement>().SetEnemyHpUI(_enemies[i]);
        }
        for (int i = 0; i < _enemies.Count; i++) 
        {
            _hpBars.transform.GetChild(i).GetComponent<EnemyHpUIElement>().SetEnemyHpUI(_enemies[i]);
        }
    }
}
