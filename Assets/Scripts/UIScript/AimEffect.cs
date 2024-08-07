using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AimEffect : MonoBehaviour
{
    [SerializeField] private RectTransform hitRateCircle;
    [SerializeField] private TMP_Text hitRateText;

    public void SetAimEffect(float hitRate)
    {
        // _image.rectTransform.rect.Set(0, 0, (hitRate * 100), (hitRate * 100));
    }
}
