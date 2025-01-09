using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectTest : MonoBehaviour
{
    RectTransform rt;
    Image img;

    int cnt = 0;
    float speed;

    Color32 imgColor = UICustomColor.HighlightStateColor;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        //rt.localScale = Vector3.zero;
        img = GetComponent<Image>();
        //img.color = new Color(imgColor.r, imgColor.g, imgColor.b, 1);
        speed = Time.deltaTime / 6.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //fade();
    }

    private void Fade()
    {
        cnt++;
        if (cnt <= 120) return;
        rt.localScale += Vector3.one * (speed * 2);
        //img.color += (new Color(0,0,0,1)) * (speed);

        if (rt.localScale.x >= 1)
        {
            rt.localScale = Vector3.zero;
            img.color = new Color(imgColor.r, imgColor.g, imgColor.b, 1);
            cnt = 0;
        }
        if (img.color.a >= .5f)
        {
        }
    }
}
