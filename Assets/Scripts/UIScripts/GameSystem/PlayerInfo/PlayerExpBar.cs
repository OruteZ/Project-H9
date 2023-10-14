using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PlayerExpBar : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _playerExpText;
    private void Start()
    {
        _playerExpText.gameObject.SetActive(false);
    }
    public void SetPlayerExpBar(float barValue, string text)
    {
        GetComponent<Slider>().value = barValue;
        _playerExpText.GetComponent<TextMeshProUGUI>().text = text;

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
