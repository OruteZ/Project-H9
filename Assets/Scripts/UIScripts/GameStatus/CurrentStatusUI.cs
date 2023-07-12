using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentStatusUI : UISystem
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _actionPointText;
    [SerializeField] private GameObject _healthPointText;
    [SerializeField] private GameObject _ConcentrationText;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.instance;
        SetCurrentStatusUI();
    }

    public override void OpenUI()
    {
    }
    public override void CloseUI()
    {
    }

    public void SetCurrentStatusUI() 
    {
        _actionPointText.GetComponent<TextMeshProUGUI>().text = _gameManager.playerStat.actionPoint.ToString();
        _healthPointText.GetComponent<TextMeshProUGUI>().text = _gameManager.playerStat.curHp.ToString();
        _ConcentrationText.GetComponent<TextMeshProUGUI>().text = _gameManager.playerStat.concentration.ToString() + "/ 100";
    }
}
