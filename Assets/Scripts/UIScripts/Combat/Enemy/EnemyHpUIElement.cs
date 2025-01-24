using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHpUIElement : UIElement
{
    private const float HP_BAR_MOVE_MAX_SPEED = 2;
    private float _hpBarMoveSpeed = HP_BAR_MOVE_MAX_SPEED;
    private float _prevEnemyHp;

    public Enemy enemy { get; private set; }
    private Vector3 _prevEnemyPos;
    private Vector3 _prevEnemyUIPos;
    
    [Header("Values")]
    [SerializeField] private float HP_BAR_UI_Y_POSITION_CORRECTION;
    [Header("References")]
    [SerializeField] private Slider _frontHpBar;
    [SerializeField] private Slider _backHpBar;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private GameObject _debuffs;
    [SerializeField] private Transform _background;
    // Start is called before the first frame update
    void Awake()
    {
        StopAllCoroutines();
        GetComponent<RectTransform>().localPosition = Vector3.zero;

        ClearEnemyHpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        //Hp Fill Setting
        float barValue = _backHpBar.value;
        LerpCalculation.CalculateLerpValue(ref barValue, _frontHpBar.value, _hpBarMoveSpeed);
        _backHpBar.value = barValue;

        //UI On & Off
        bool isUIVisible;
        if (enemy == null)
        {
            isUIVisible = (_backHpBar.value > 0);
            if (!isUIVisible)
            {
                ClearEnemyHpUI();
            }
        }
        else
        {
            isUIVisible = enemy.meshVisible;
        }
        _backHpBar.gameObject.SetActive(isUIVisible);
        _frontHpBar.gameObject.SetActive(isUIVisible);
        _hpText.gameObject.SetActive(isUIVisible);
        _debuffs.gameObject.SetActive(isUIVisible);
        _background.gameObject.SetActive(isUIVisible);

        //UI Position Setting
        if (enemy != null) 
        {
            _prevEnemyPos = enemy.transform.position;
        } 
        Vector3 enemyPositionHeightCorrection = _prevEnemyPos;
        enemyPositionHeightCorrection.y += 1.8f;
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(enemyPositionHeightCorrection);
        if (uiPosition != _prevEnemyUIPos)
        {
            uiPosition.y += HP_BAR_UI_Y_POSITION_CORRECTION;
            GetComponent<RectTransform>().position = uiPosition;

            _prevEnemyUIPos = uiPosition;
        }
    }

    public void InitEnemyHpUI(Enemy enemy)
    {
        this.enemy = enemy;
        SetEnemyHpUI();
    }
    /// <summary>
    /// �� ü�¹��� ������ �����մϴ�.
    /// EnemyHpUI�� ��� �� ü�¹� �������� ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="enemy"> �ش� ü�¹ٷ� ������ �� ��ü </param>
    public void SetEnemyHpUI()
    {
        if (enemy == null) return;
        _prevEnemyUIPos = Vector3.zero;
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
        if (this.enemy == null)
        {
            _backHpBar.value = curHp;
            _prevEnemyHp = curHp;
        }
        if (_prevEnemyHp != curHp) 
        {
            StartCoroutine(DelaySliderMove());
            _prevEnemyHp = curHp;
        }

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

        gameObject.SetActive(true);
    }
    private void ClearEnemyHpUI()
    {
        gameObject.SetActive(false);
        GetComponent<RectTransform>().localPosition = Vector3.zero;
        enemy = null;
        UIManager.instance.combatUI.enemyHpUI.onEnemyHpDeleted.Invoke(gameObject);
    }

    private IEnumerator DelaySliderMove() 
    {
        _hpBarMoveSpeed = 0;
        yield return new WaitForSeconds(.5f);
        _hpBarMoveSpeed = HP_BAR_MOVE_MAX_SPEED;
        yield break;
    }
}
