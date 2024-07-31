using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemListUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _itemIcon;
    [SerializeField] private GameObject _itemNameText;
    [SerializeField] private GameObject _itemPriceText;
    [SerializeField] private GameObject _itemEffect;

    private ItemData _itemData = null;
    private ItemListUI _listUI;

    private bool _doubleClickTrigger = false;
    private Color _targetColor = Color.white;
    private void Awake()
    {
        _targetColor.a = 0;
    }
    public void SetItemListUIElement(ItemData iData, ItemListUI listUI) 
    {
        _itemData = iData;
        _listUI = listUI;
        _itemIcon.GetComponent<Image>().sprite = iData.icon;
        _itemNameText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.itemDatabase.GetItemScript(iData.nameIdx).GetName();
        _itemPriceText.GetComponent<TextMeshProUGUI>().text = iData.itemPrice.ToString()+ '¡Ë';

        if (GameManager.instance.playerInventory.GetGold() < iData.itemPrice) _itemPriceText.GetComponent<TextMeshProUGUI>().color = Color.red;
        else _itemPriceText.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.x += GetComponent<RectTransform>().sizeDelta.x / 2 * UIManager.instance.GetCanvasScale();
        pos.y += GetComponent<RectTransform>().sizeDelta.y / 2 * UIManager.instance.GetCanvasScale();
        _listUI.SetItemListTooltip(_itemData, pos);

        _targetColor.a = 1 / 4.0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _listUI.CloseItemListTooltip();
        _targetColor.a = 0;
    }
    private void Update()
    {
        Color c = _itemEffect.GetComponent<Image>().color;
        LerpCalculation.CalculateLerpValue(ref c.a, _targetColor.a, 70);
        _itemEffect.GetComponent<Image>().color = c;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_doubleClickTrigger)
        {
            _doubleClickTrigger = true;
            StartCoroutine(DoubleClickResetTimer());
        }
        else
        {
            StopAllCoroutines();
            _doubleClickTrigger = false;
            _listUI.BuyItem(_itemData);
        }
        StopCoroutine(ClickAlphaTimer());
        StartCoroutine(ClickAlphaTimer());
    }
    IEnumerator ClickAlphaTimer()
    {
        _targetColor.a = 0;
        yield return new WaitForSeconds(0.1f);
        _targetColor.a = 1 / 4.0f;
        yield break;
    }
    IEnumerator DoubleClickResetTimer() 
    {
        yield return new WaitForSeconds(0.2f);
        _doubleClickTrigger = false;
        yield break;
    }

}
