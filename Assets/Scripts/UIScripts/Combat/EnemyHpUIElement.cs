using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHpUIElement : UIElement
{
    private Slider _frontHpBar;
    private Slider _backHpBar;
    private TextMeshProUGUI _hpText;

    private Enemy _enemy;

    [SerializeField] private float HP_BAR_UI_Y_POSITION_CORRECTION =50;
    // Start is called before the first frame update
    void Awake()
    {
        _backHpBar = transform.GetChild(0).GetComponent<Slider>();
        _frontHpBar = transform.GetChild(1).GetComponent<Slider>();
        _hpText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Debug.Log(_hpText.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemy is not null)
        {
            _backHpBar.gameObject.SetActive(_enemy.isVisible);
            _frontHpBar.gameObject.SetActive(_enemy.isVisible);
            _hpText.gameObject.SetActive(_enemy.isVisible);
            if (!_enemy.isVisible) return;

            Vector3 uiPosition = Camera.main.WorldToScreenPoint(_enemy.transform.position);
            uiPosition.y += HP_BAR_UI_Y_POSITION_CORRECTION;
            GetComponent<RectTransform>().position = uiPosition;

            if (_frontHpBar.value != _backHpBar.value)
            {
                _backHpBar.value = Mathf.Lerp(_backHpBar.value, _frontHpBar.value, Time.deltaTime * 2);
            }
        }
    }

    public void SetEnemyHpUI(Enemy enemy)
    {
        _enemy = enemy;
        int maxHp = enemy.GetStat().maxHp;
        int curHp = enemy.GetStat().curHp;

        _frontHpBar.maxValue = maxHp;
        _frontHpBar.minValue = 0;
        _backHpBar.maxValue = maxHp;
        _backHpBar.minValue = 0;

        _hpText.text = curHp.ToString() + " / " + maxHp.ToString();
        _frontHpBar.value = curHp;
    }
    public void ClearEnemyHpUI()
    {
        _enemy = null;
    }
}
