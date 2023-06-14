using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemListElement : MonoBehaviour
{
    private Image ItemIcon;
    private TextMeshProUGUI ItemName;
    // Start is called before the first frame update
    void Awake()
    {
        ItemIcon = this.transform.GetChild(0).GetComponent<Image>();
        ItemName = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetItemListElement(Item item)
    {
        /*
        Sprite sprite = Resources.Load("Images/" + item.itemInfo.iconNumber) as Sprite;
        ItemIcon.sprite = sprite;
        */
        ItemName.text = item.itemInfo.name;
    }
}
