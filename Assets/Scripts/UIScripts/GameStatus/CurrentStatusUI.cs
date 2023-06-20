using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentStatusUI : UISystem
{
    public GameManager gameManager;

    [SerializeField] private GameObject _actionPointText;
    [SerializeField] private GameObject _healthPointText;
    [SerializeField] private GameObject _ConcentrationText;
    [SerializeField] private GameObject _ConcentrationSlider;

    // Start is called before the first frame update
    void Start()
    {
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
        _actionPointText.GetComponent<TextMeshProUGUI>().text = gameManager.playerStat.actionPoint.ToString();
        _healthPointText.GetComponent<TextMeshProUGUI>().text = gameManager.playerStat.hp.ToString();
        _ConcentrationText.GetComponent<TextMeshProUGUI>().text = gameManager.playerStat.concentration.ToString() + "/ 100";
        _ConcentrationSlider.GetComponent<Slider>().value = gameManager.playerStat.concentration / 100.0f;
    }
}
