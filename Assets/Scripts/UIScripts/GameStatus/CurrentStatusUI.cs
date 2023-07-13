using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentStatusUI : UISystem
{
    private UnitStat _playerStat;

    [SerializeField] private GameObject _actionPointText;
    [SerializeField] private GameObject _healthPointText;
    [SerializeField] private GameObject _ConcentrationText;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentStatusUI();
    }
    private void Update()
    {
        SetCurrentStatusUI();//test
    }
    public override void OpenUI()
    {
    }
    public override void CloseUI()
    {
    }

    public void SetCurrentStatusUI()
    {
        _playerStat = GameManager.instance.playerStat;
        _healthPointText.GetComponent<TextMeshProUGUI>().text = _playerStat.curHp.ToString() + " / " + _playerStat.maxHp.ToString();
        _ConcentrationText.GetComponent<TextMeshProUGUI>().text = _playerStat.concentration.ToString();
        _actionPointText.GetComponent<TextMeshProUGUI>().text = _playerStat.actionPoint.ToString()+" / "/*+_playerStat.maxActionPoint.ToString()*/;
    }
}
