using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUIElement : MonoBehaviour
{
    private bool isFill;

    private void Start()
    {
        isFill = false;
    }
    public void FillUI() 
    {
        GetComponent<Image>().color = new Color(1, 0, 0, 1);
        isFill = true;
    }

    public void EmptyUI()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        isFill = false;
    }
}
