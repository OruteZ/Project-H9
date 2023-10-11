using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerExpUI : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _playerExpText;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.onPlayerStatChanged.AddListener(SetPlayerExpUI);
        SetPlayerExpUI();
        _playerExpText.gameObject.SetActive(false);
    }

    public void SetPlayerExpUI()
    {
        int maxExp = GameManager.instance.GetMaxExp();
        int curExp = GameManager.instance.curExp;
        float expSliderValue = (float)curExp / maxExp;

        Debug.Log(expSliderValue);
        GetComponent<Slider>().value = expSliderValue;
        _playerExpText.GetComponent<TextMeshProUGUI>().text = curExp.ToString() + " / " + maxExp.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _playerExpText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _playerExpText.gameObject.SetActive(false);
    }
}
