using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어의 Hp아이콘UI 표시 방식을 구현하는 클래스
/// </summary>
public class PlayerHpUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _playerHpBar;
    [SerializeField] private GameObject _playerHpText;

    [SerializeField] private GameObject _hpTooltip;

    //[SerializeField] private GameObject _playerHpContainer;
    //[SerializeField] private GameObject hpUIPrefab;

    //private List<GameObject> _hpUIs = new List<GameObject>();
    //private readonly Vector3 HP_UI_INIT_POSITION = new Vector3(-120, 0, 0);
    //private readonly int HP_UI_INTERVAL = 25;
    private int _prevMaxHp;
    private int _prevCurHp;
    void Start()
    {
        _hpTooltip?.SetActive(false);
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
        _prevMaxHp = 0;
        _prevCurHp = 0;

        //int maxHp = GameManager.instance.playerStat.GetStat(StatType.MaxHp);
        //HpUIObjectPooling(maxHp);
        SetHpUI();
    }
    public void SetHpUI()
    {
        int maxHp = GameManager.instance.playerStat.GetStat(StatType.MaxHp);
        int curHp = GameManager.instance.playerStat.GetStat(StatType.CurHp);
        if (maxHp == _prevMaxHp && curHp == _prevCurHp) return;

        _playerHpBar.GetComponent<Image>().fillAmount = (float)curHp / maxHp;
        _playerHpText.GetComponent<TextMeshProUGUI>().text = curHp.ToString() + " / " + maxHp.ToString();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _hpTooltip?.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _hpTooltip?.SetActive(false);
    }

    //private void HpUIObjectPooling(int length)
    //{
    //    for (int i = 0; i < length; i++)
    //    {
    //        Vector3 pos = CalculateHpUIPosition(i);
    //        GameObject ui = Instantiate(hpUIPrefab, pos, Quaternion.identity, _playerHpContainer.transform);
    //        ui.GetComponent<PlayerHpUIElement>().FillUI();
    //        _hpUIs.Add(ui);
    //        ui.SetActive(false);
    //    }
    //    _prevMaxHp = length;
    //}

    ///// <summary>
    ///// 플레이어 Hp아이콘UI를 배치합니다.
    ///// CurrentStatusUI가 갱신될 때 같이 갱신됩니다.
    ///// </summary>
    //public void SetHpUI()
    //{
    //    int maxHp = GameManager.instance.playerStat.GetStat(StatType.MaxHp);
    //    int curHp = GameManager.instance.playerStat.GetStat(StatType.CurHp);
    //    // int maxHp = FieldSystem.unitSystem.GetPlayer().GetStat().maxHp;
    //    // int curHp = FieldSystem.unitSystem.GetPlayer().GetStat().curHp;
    //    if (maxHp == _prevMaxHp && curHp == _prevCurHp) return;

    //    if (maxHp > _prevMaxHp) 
    //    {
    //        HpUIObjectPooling(10);
    //    }

    //    for (int i = 0; i < _hpUIs.Count; i++) 
    //    {
    //        if (i < maxHp)
    //        {
    //            _hpUIs[i].SetActive(true);
    //            _hpUIs[i].transform.localPosition = CalculateHpUIPosition(i);
    //            if (i < curHp)
    //            {
    //                _hpUIs[i].GetComponent<PlayerHpUIElement>().FillUI();
    //            }
    //            else
    //            {
    //                _hpUIs[i].GetComponent<PlayerHpUIElement>().EmptyUI();
    //            }
    //        }
    //        else
    //        {
    //            _hpUIs[i].SetActive(false);
    //        }
    //    }

    //    _prevMaxHp = maxHp;
    //    _prevCurHp = curHp;
    //}

    //private Vector3 CalculateHpUIPosition(int i)
    //{
    //    Vector3 pos = HP_UI_INIT_POSITION;
    //    pos.x += HP_UI_INTERVAL * (i % 10);
    //    pos.y += HP_UI_INTERVAL * (i / 10);
    //    return pos;
    //}
}
