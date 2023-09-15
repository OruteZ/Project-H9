using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 캐릭터 정보 창에서 인벤토리에 위치한 아이템 각각을 구현한 클래스
/// </summary>
public class ItemListElement : MonoBehaviour
{
    private Image _ItemIcon;
    private TextMeshProUGUI _ItemName;
    private int _itemIndex;

    // Start is called before the first frame update
    void Awake()
    {
        _ItemIcon = this.transform.GetChild(0).GetComponent<Image>();
        _ItemName = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// 아이템 리스트 UI의 각 아이템 UI 정보를 갱신합니다.
    /// itemListUI가 아이템 리스트 전체를 갱신할 때 실행됩니다.
    /// </summary>
    /// <param name="item"> 아이템 UI가 표시할 아이템 </param>
    public void SetItemListElement(Item item)
    {
        _itemIndex = item.itemInfo.index;
        /*
        Sprite sprite = Resources.Load("Images/" + item.itemInfo.iconNumber) as Sprite;
        ItemIcon.sprite = sprite;
        */
        _ItemName.text = item.itemInfo.name;
    }

    /// <summary>
    /// 아이템 UI를 클릭했을 때 팝업창을 띄우라는 명령을 itemListUI에게 보냅니다.
    /// </summary>
    public void OnItemUIBtnClick()
    {
        UIManager.instance.characterUI.itemListUI.ClickItemUIButton(_itemIndex);
    }
}
