using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ¸íÁß·ü µð¹ö±ëÀ» À§ÇÑ µð¹ö±ë UI Å¬·¡½º
/// </summary>
public class HitRateDebugWindow : UISystem
{
    [SerializeField] private GameObject _text;
    private bool _isOpened;

    Unit _target;
    // Start is called before the first frame update
    void Start()
    {
        _isOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public override void OpenUI() 
    {
        _isOpened = true;
    }
    public override void CloseUI()
    {
        _isOpened = false;
    }
    public void SetText(float hitRate, Unit unit, Unit target, float dist, float wRange, float addRange, float penalty)
    {
        if (_isOpened)
        {
            _text.GetComponent<TextMeshProUGUI>().text =
                hitRate.ToString() + '\n' +
                unit.unitName.ToString() + '\n' +
                target.unitName.ToString() + '\n' +
                dist.ToString() + '\n' +
                wRange.ToString() + '\n' +
                addRange.ToString() + '\n' +
                penalty.ToString();
        }
    }
}
