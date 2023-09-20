using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� Hp������UI ǥ�� ����� �����ϴ� Ŭ����
/// </summary>
public class PlayerHpUI : UISystem
{
    [SerializeField] private GameObject _playerHpContainer;
    [SerializeField] private GameObject hpUIPrefab;

    private List<GameObject> _hpUIs = new List<GameObject>();
    private readonly Vector3 HP_UI_INIT_POSITION = new Vector3(-120, 0, 0);
    private readonly int HP_UI_INTERVAL = 25;
    private int _prevMaxHp;
    private int _prevCurHp;
    void Start()
    {
        InitHPUI();
    }
    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }

    private void InitHPUI()
    {
        int maxHp = GameManager.instance.playerStat.maxHp;
        _prevMaxHp = 0;
        _prevCurHp = 0;

        HpUIObjectPooling(maxHp);
        SetHpUI();
    }
    private void HpUIObjectPooling(int length)
    {
        for (int i = 0; i < length; i++)
        {
            Vector3 pos = CalculateHpUIPosition(i);
            GameObject ui = Instantiate(hpUIPrefab, pos, Quaternion.identity, _playerHpContainer.transform);
            ui.GetComponent<PlayerHpUIElement>().FillUI();
            _hpUIs.Add(ui);
            ui.SetActive(false);
        }
        _prevMaxHp = length;
    }

    /// <summary>
    /// �÷��̾� Hp������UI�� ��ġ�մϴ�.
    /// CurrentStatusUI�� ���ŵ� �� ���� ���ŵ˴ϴ�.
    /// </summary>
    public void SetHpUI()
    {
        int maxHp = GameManager.instance.playerStat.maxHp;
        int curHp = GameManager.instance.playerStat.curHp;
        // int maxHp = FieldSystem.unitSystem.GetPlayer().GetStat().maxHp;
        // int curHp = FieldSystem.unitSystem.GetPlayer().GetStat().curHp;
        if (maxHp == _prevMaxHp && curHp == _prevCurHp) return;

        if (maxHp > _prevMaxHp) 
        {
            HpUIObjectPooling(10);
        }

        for (int i = 0; i < _hpUIs.Count; i++) 
        {
            if (i < maxHp)
            {
                _hpUIs[i].SetActive(true);
                _hpUIs[i].transform.localPosition = CalculateHpUIPosition(i);
                if (i < curHp)
                {
                    _hpUIs[i].GetComponent<PlayerHpUIElement>().FillUI();
                }
                else
                {
                    _hpUIs[i].GetComponent<PlayerHpUIElement>().EmptyUI();
                }
            }
            else
            {
                _hpUIs[i].SetActive(false);
            }
        }

        _prevMaxHp = maxHp;
        _prevCurHp = curHp;
    }

    private Vector3 CalculateHpUIPosition(int i)
    {
        Vector3 pos = HP_UI_INIT_POSITION;
        pos.x += HP_UI_INTERVAL * (i % 10);
        pos.y += HP_UI_INTERVAL * (i / 10);
        return pos;
    }
}