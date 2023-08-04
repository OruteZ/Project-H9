using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUIElement : MonoBehaviour
{
    public void FillUI()
    {
        GetComponent<Image>().color = new Color(1, 0, 0, 1);
        Debug.Log("Fill");
    }

    public void EmptyUI()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        Debug.Log("Empty");
    }
}
