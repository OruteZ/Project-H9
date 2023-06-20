using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemListElement : MonoBehaviour
{
    private Image _ItemIcon;
    private TextMeshProUGUI _ItemName;
    private int _itemIndex;
    private ItemListUI _itemListUI;
    // Start is called before the first frame update
    void Awake()
    {
        _ItemIcon = this.transform.GetChild(0).GetComponent<Image>();
        _ItemName = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetItemListElement(Item item, ItemListUI itemUI)
    {
        _itemIndex = item.itemInfo.index;
        /*
        Sprite sprite = Resources.Load("Images/" + item.itemInfo.iconNumber) as Sprite;
        ItemIcon.sprite = sprite;
        */
        _ItemName.text = item.itemInfo.name;
        _itemListUI = itemUI;
    }

    public void OnItemUIBtnClick()
    {
        _itemListUI.ClickCharacterUIButton(_itemIndex);
    }
}
