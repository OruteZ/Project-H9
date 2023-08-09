using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpUI : UISystem
{
    [SerializeField] private GameObject _hpBarPrefab;
    [SerializeField] private GameObject _enemyhpBarUIs;

    private List<GameObject> _enemyHpBars;

    private List<Enemy> _enemies = new List<Enemy>();

    void Start()
    {
        _enemyHpBars = new List<GameObject>();
        EnemyHpBarObjectPooling(10);
    }

    public void SetEnemyHpBars()
    {
        List<Unit> units = FieldSystem.unitSystem.units;
        _enemies = new List<Enemy>();
        foreach (Unit unit in units)
        {
//            Debug.Log("type:" + unit.GetType());
            if (unit is Enemy)
            {
                _enemies.Add((Enemy)unit);
            }
        }

//        Debug.Log("적 개체 수:" + _enemies.Count);

        if (_enemies.Count > _enemyHpBars.Count) 
        {
            EnemyHpBarObjectPooling(10);
        }
        InitEnemyHpUIs();
        for (int i = 0; i < _enemies.Count; i++) 
        {
            _enemyHpBars[i].SetActive(true);
            _enemyHpBars[i].GetComponent<EnemyHpUIElement>().SetEnemyHpUI(_enemies[i]);
        }
    }
    private void InitEnemyHpUIs()
    {
        foreach (GameObject hpBar in _enemyHpBars)
        {
            hpBar.SetActive(false);
        }
    }
    private void EnemyHpBarObjectPooling(int length)
    {
        for (int i = 0; i < length; i++)
        {
            GameObject ui = Instantiate(_hpBarPrefab, Vector3.zero, Quaternion.identity, _enemyhpBarUIs.transform);
            _enemyHpBars.Add(ui);
        }
        InitEnemyHpUIs();
    }
}
