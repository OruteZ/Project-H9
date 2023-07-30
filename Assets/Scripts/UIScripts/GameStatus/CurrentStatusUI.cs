using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentStatusUI : UISystem
{
    private UnitStat _playerStat;
    private Unit _player;

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
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }

    public void SetCurrentStatusUI()
    {
        _player = FieldSystem.unitSystem.GetPlayer();
        //_playerStat = _player.GetStat();
        _playerStat = GameManager.instance.playerStat;
        
        _healthPointText.GetComponent<TextMeshProUGUI>().text = _playerStat.curHp.ToString() + " / " + _playerStat.maxHp.ToString();
        _ConcentrationText.GetComponent<TextMeshProUGUI>().text = _playerStat.concentration.ToString();
        _actionPointText.GetComponent<TextMeshProUGUI>().text = _player.currentActionPoint.ToString() + " / " + _playerStat.actionPoint.ToString();
        if (_player.currentActionPoint == 0)
        {
            _actionPointText.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            _actionPointText.GetComponent<TextMeshProUGUI>().color = Color.black;
        }

        GetComponent<PlayerHpUI>().SetHPUI();
    }
}
