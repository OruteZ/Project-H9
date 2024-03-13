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
    private GameObject _debuffs;
    private float HP_BAR_MOVE_SPEED = 2;

    private Enemy _enemy;
    private Vector3 _enemyUIPrevPos;

    [SerializeField] private float HP_BAR_UI_Y_POSITION_CORRECTION;
    // Start is called before the first frame update
    void Awake()
    {
        _backHpBar = transform.GetChild(0).GetComponent<Slider>();
        _frontHpBar = transform.GetChild(1).GetComponent<Slider>();
        _hpText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _debuffs = transform.GetChild(3).gameObject;
        GetComponent<RectTransform>().position = Vector3.zero;

        ClearEnemyHpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        //Hp Fill Setting
        float barValue = _backHpBar.value;
        LerpCalculation.CalculateLerpValue(ref barValue, _frontHpBar.value, HP_BAR_MOVE_SPEED);
        _backHpBar.value = barValue;

        //UI On & Off
        bool isUIVisible;
        if (_enemy == null)
        {
            isUIVisible = (_backHpBar.value > 0);
            if (!isUIVisible) 
            {
                ClearEnemyHpUI();
            }      
        }
        else
        {
            isUIVisible = _enemy.isVisible;
        }
        _backHpBar.gameObject.SetActive(isUIVisible);
        _frontHpBar.gameObject.SetActive(isUIVisible);
        _hpText.gameObject.SetActive(isUIVisible);
        _debuffs.gameObject.SetActive(isUIVisible);

        //UI Position Setting
        if (_enemy == null) return;
        Vector3 enemyPositionHeightCorrection = _enemy.transform.position;
        enemyPositionHeightCorrection.y += 1.8f;
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(enemyPositionHeightCorrection);
        if (uiPosition != _enemyUIPrevPos)
        {
            uiPosition.y += HP_BAR_UI_Y_POSITION_CORRECTION;
            GetComponent<RectTransform>().position = uiPosition;

            _enemyUIPrevPos = uiPosition;
        }
    }

    /// <summary>
    /// 적 체력바의 정보를 설정합니다.
    /// EnemyHpUI가 모든 적 체력바 정보들을 설정할 때 호출됩니다.
    /// </summary>
    /// <param name="enemy"> 해당 체력바로 설정할 적 개체 </param>
    public void SetEnemyHpUI(Enemy enemy)
    {

        _enemyUIPrevPos = Vector3.zero;
        int maxHp = enemy.stat.maxHp;
        int curHp = enemy.stat.curHp;

        _frontHpBar.maxValue = maxHp;
        _frontHpBar.minValue = 0;
        _backHpBar.maxValue = maxHp;
        _backHpBar.minValue = 0;

        int curHpText = curHp;
        if (curHpText < 0) curHpText = 0;
        _hpText.text = curHpText.ToString() + " / " + maxHp.ToString();
        _frontHpBar.value = curHp;

        IDisplayableEffect[] debuffList = enemy.GetDisplayableEffects();
        for (int i = 0; i < _debuffs.transform.childCount; i++)
        {
            if (debuffList.Length > i)
            {
                _debuffs.transform.GetChild(i).gameObject.SetActive(true);
                _debuffs.transform.GetChild(i).GetComponent<BuffUIElement>().SetBuffUIElement(debuffList[i], false, false);
            }
            else
            {
                _debuffs.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        _enemy = enemy;
        gameObject.SetActive(true);
    }
    public void ClearEnemyHpUI()
    {
        gameObject.SetActive(false);
        GetComponent<RectTransform>().position = Vector3.zero;
        _enemy = null;
    }
}
