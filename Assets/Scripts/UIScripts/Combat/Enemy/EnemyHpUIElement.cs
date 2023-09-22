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
    private Vector3 _enemyUIPrevPos;

    [SerializeField] private float HP_BAR_UI_Y_POSITION_CORRECTION;
    // Start is called before the first frame update
    void Awake()
    {
        _backHpBar = transform.GetChild(0).GetComponent<Slider>();
        _frontHpBar = transform.GetChild(1).GetComponent<Slider>();
        _hpText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        _enemy = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemy == null) 
        {
            ClearEnemyHpUI();
            return;
        }

        //UI On & Off
        _backHpBar.gameObject.SetActive(_enemy.isVisible);
        _frontHpBar.gameObject.SetActive(_enemy.isVisible);
        _hpText.gameObject.SetActive(_enemy.isVisible);
        if (!_enemy.isVisible) return;

        //UI Position Setting
        Vector3 enemyPositionHeightCorrection = _enemy.transform.position;
        enemyPositionHeightCorrection.y += 1.8f;
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(enemyPositionHeightCorrection);
        if (uiPosition != _enemyUIPrevPos)
        {
            uiPosition.y += HP_BAR_UI_Y_POSITION_CORRECTION;
            GetComponent<RectTransform>().position = uiPosition;

            _enemyUIPrevPos = uiPosition;
        }

        //Hp Fill Setting
        float threshold = 0.01f;
        if (Mathf.Abs(_frontHpBar.value - _backHpBar.value) > threshold)
        {
            _backHpBar.value = Mathf.Lerp(_backHpBar.value, _frontHpBar.value, Time.deltaTime * 2);
        }
        else
        {
            _frontHpBar.value = _backHpBar.value;
        }
    }

    /// <summary>
    /// 적 체력바의 정보를 설정합니다.
    /// EnemyHpUI가 모든 적 체력바 정보들을 설정할 때 호출됩니다.
    /// </summary>
    /// <param name="enemy"> 해당 체력바로 설정할 적 개체 </param>
    public void SetEnemyHpUI(Enemy enemy)
    {
        gameObject.SetActive(true);

        _enemyUIPrevPos = Vector3.zero;
        int maxHp = enemy.GetStat().maxHp;
        int curHp = enemy.GetStat().curHp;

        _frontHpBar.maxValue = maxHp;
        _frontHpBar.minValue = 0;
        _backHpBar.maxValue = maxHp;
        _backHpBar.minValue = 0;

        int curHpText = curHp;
        if (curHpText < 0) curHpText = 0;
        _hpText.text = curHpText.ToString() + " / " + maxHp.ToString();
        _frontHpBar.value = curHp;

        _enemy = enemy;
    }
    public void ClearEnemyHpUI()
    {
        gameObject.SetActive(false);
        _enemy = null;
    }
}
