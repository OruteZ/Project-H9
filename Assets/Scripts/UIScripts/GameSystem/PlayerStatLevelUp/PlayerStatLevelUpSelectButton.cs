using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerStatLevelUpSelectButton : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _frame;
    [SerializeField] private Image _checkmark;
    private Color _color = new Color32(200, 116, 57, 255);

    private bool _isMouseOver = false;
    private float _scaleCorrection = 1.0f;
    private float _scaleCorrectSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color color = new Color(_color.r / 2, _color.g / 2, _color.b / 2, 1.0f);
        _scaleCorrection = 1.0f;
        if (_isMouseOver)
        {
            color = _color;
            if (Input.GetMouseButton(0))
            {
                _scaleCorrection *= 0.95f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                UIManager.instance.gameSystemUI.playerStatLevelUpUI.ClickSelectButton();
            }
        }

        //color setting
        _frame.color = new Color(color.r, color.g, color.b, color.a / 2);
        _checkmark.color = color;

        //scale correction
        Vector3 scale = GetComponent<RectTransform>().localScale;
        float threshold = 0.001f;
        if (Mathf.Abs(scale.x - _scaleCorrection) > threshold)
        {
            scale.x = Mathf.Lerp(scale.x, _scaleCorrection, Time.deltaTime * _scaleCorrectSpeed);
        }
        else
        {
            scale.x = _scaleCorrection;
        }
        scale.y = scale.x;
        GetComponent<RectTransform>().localScale = scale;
    }
    public void InitPlayerStatLevelUpSelectButton()
    {
        Color color = new Color(_color.r / 2, _color.g / 2, _color.b / 2, 1.0f);
        _frame.color = new Color(color.r, color.g, color.b, color.a / 2);
        _checkmark.color = color;

        _isMouseOver = false;
        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
    }
}
