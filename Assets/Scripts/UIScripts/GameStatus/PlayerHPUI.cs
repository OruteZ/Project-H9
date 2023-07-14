using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPUI : UISystem
{
    [SerializeField] private GameObject _playerHps;
    public GameObject hpUIPrefabs;

    private List<GameObject> _hpUIs;
    private readonly Vector3 HP_UI_INIT_POSITION = new Vector3(50, 210, 0);
    private readonly int HP_UI_INTERVAL = 30;
    private int prevMaxHp;
    private int prevCurHp;

    void Start()
    {
        InitHPUI();
    }

    private void InitHPUI()
    {
        _hpUIs = new List<GameObject>();
        int maxHp = GameManager.instance.playerStat.maxHp;
        int curHp = GameManager.instance.playerStat.curHp;

        HpUIObjectPooling(maxHp);
        SetHPUI();
    }
    private void HpUIObjectPooling(int leng)
    {
        for (int i = 0; i < leng; i++)
        {
            Vector3 pos = CalculateHpUIPosition(i);
            GameObject ui = Instantiate(hpUIPrefabs, pos, Quaternion.identity, _playerHps.transform);
            ui.SetActive(false);
            _hpUIs.Add(ui);
        }
        prevMaxHp = leng;
    }

    public void SetHPUI()
    {
        int maxHp = GameManager.instance.playerStat.maxHp;
        int curHp = GameManager.instance.playerStat.curHp;
        if (maxHp == prevMaxHp && curHp == prevCurHp) return;

        if (maxHp > prevMaxHp) 
        {
            HpUIObjectPooling(10);
        }

        for (int i = 0; i < _hpUIs.Count; i++) 
        {
            if (i < maxHp)
            {
                _hpUIs[i].SetActive(true);
                _hpUIs[i].transform.position = CalculateHpUIPosition(i);
                if (i < curHp)
                {
                    _hpUIs[i].GetComponent<HPUIElement>().FillUI();
                }
                else
                {
                    _hpUIs[i].GetComponent<HPUIElement>().EmptyUI();
                }
            }
            else
            {
                _hpUIs[i].SetActive(false);
            }
        }

        prevMaxHp = maxHp;
        prevCurHp = curHp;
    }

    private Vector3 CalculateHpUIPosition(int i)
    {
        Vector3 pos = HP_UI_INIT_POSITION;
        pos.x += HP_UI_INTERVAL * (i % 10);
        pos.y += HP_UI_INTERVAL * (i / 10);
        return pos;
    }
}
