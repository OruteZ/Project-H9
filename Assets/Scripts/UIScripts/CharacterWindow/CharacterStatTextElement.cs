using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterStatTextElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _contentsText;

    private bool _isSetContents = false;
    private bool _isOpenTooltip = false;
    private bool _isMouseOver = false;
    private float _mouseOverCount = 0.0f;

    private string _currentStatName;
    private CharacterTooltip _characterTooltip;
    // Start is called before the first frame update
    void Start()
    {
        _nameText = GetComponent<TextMeshProUGUI>();
        _contentsText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMouseOver && !_isOpenTooltip && _isSetContents) 
        {
            _mouseOverCount += Time.deltaTime;
            if (_mouseOverCount > 0.5f) 
            {
                _isOpenTooltip = true;
                _characterTooltip.SetCharacterTooltip(_currentStatName, GetComponent<RectTransform>().position.y);
            }
        }
    }
    public void SetCharacterStatText(string name, string value) 
    {
        _currentStatName = name.Replace(':', '\0');
        _nameText.text = name;
        _contentsText.text = value;
        _isSetContents = (_currentStatName != "");
        _characterTooltip = UIManager.instance.characterUI.characterStatUI._characterStatTooltip.GetComponent<CharacterTooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _nameText.color = Color.yellow;
        _contentsText.color = Color.yellow;

        _isOpenTooltip = false;
        _isMouseOver = true;
        _mouseOverCount = 0.0f;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _nameText.color = Color.white;
        _contentsText.color = Color.white;

        _isOpenTooltip = false;
        _isMouseOver = false;
        _mouseOverCount = 0.0f;

        _characterTooltip.CloseUI();
    }
}
