using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 전투 시 적의 스텟을 확인할 수 있는 적 스텟창 UI를 표시하는 기능을 수행하는 클래스
/// </summary>
public class EnemyStatUI : UISystem
{
    [SerializeField] private GameObject _mouseOverIcon;
    [SerializeField] private GameObject _enemyStatWindow;

    [SerializeField] private GameObject _enemyNameText;
    [SerializeField] private GameObject _enemyInfoUI;

    private const float WINDOW_X_POSITION_CORRECTION = 300;

    private bool _isOpenedTooltipWindow = false;
    private Enemy _statOpenEnemy = null;
    private Enemy _mouseOverEnemy =  null;

    private bool _isOpenInfoPopupOnce = false;
    public override void OpenPopupWindow()
    {
        _isOpenedTooltipWindow = true;
        UIManager.instance.SetUILayer(3);
        _enemyStatWindow.SetActive(true);
    }
    public override void ClosePopupWindow()
    {
        _isOpenedTooltipWindow = false;
        CloseEnemyStatUI();
        UIManager.instance.SetUILayer(1);
    }
    private void Update()
    {
        if (!GameManager.instance.CompareState(GameState.Combat)) return;
        if (FieldSystem.turnSystem.turnOwner is Player)
        {
            bool isMouseover = IsMouseOverOnEnemy(out Vector3Int ep);
            _mouseOverIcon.SetActive(isMouseover);
            if (isMouseover)
            {
                Vector3 pos = FieldSystem.tileSystem.GetTile(ep).transform.position;
                _mouseOverIcon.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(pos);
            }
        }
        else
        {
            _mouseOverIcon.SetActive(false);
        }

        if (IsMouseOverOnEnemy(out Vector3Int enemyPos))
        {
            Player player = FieldSystem.unitSystem.GetPlayer();
            Enemy enemy = (Enemy)FieldSystem.unitSystem.GetUnit(enemyPos);
            if (player is null || enemy is null) return;

            enemy.TryGetComponent(out CustomOutline outline);
            if (outline is null)
            {
                outline = enemy.gameObject.AddComponent<CustomOutline>();
                outline.OutlineColor = Color.red;
            }
            outline.OutlineMode = CustomOutline.Mode.OutlineAll;

            if (Input.GetMouseButtonDown(1))
            {
                if (player.GetSelectedAction().GetActionType() is not ActionType.IDLE) return;
                if (enemy.isVisible)
                {
                    SetEnemyStatUI(enemy);
                }
            }

            if (_mouseOverEnemy != enemy)
            {
                UserData user = GameManager.instance.user;
                if (!user.Events.TryGetValue("INFO_POPUP_HIT_RATE", out var value) || value == 0)
                {
                    if (!_isOpenInfoPopupOnce && player.GetSelectedAction() is AttackAction)
                    {
                        _isOpenInfoPopupOnce = true;
                        InfoPopup.instance.Show(InfoPopup.MESSAGE.IT_IS_HIT_RATE);
                        user.Events.TryAdd("INFO_POPUP_HIT_RATE", 1);
                    }
                }
                _mouseOverEnemy = enemy;
                UIManager.instance.combatUI.turnOrderUI.EffectMouseOverEnemy(enemy);
            }
        }
        else if(_mouseOverEnemy != null)
        {
            _mouseOverEnemy.TryGetComponent(out CustomOutline outline);
            if (outline is null)
            {
                outline = _mouseOverEnemy.gameObject.AddComponent<CustomOutline>();
                outline.OutlineColor = Color.red;
            }
            outline.OutlineMode = CustomOutline.Mode.NULL;

            _mouseOverEnemy = null;
            UIManager.instance.combatUI.turnOrderUI.EffectMouseOverEnemy(null);
        }

        if (_isOpenedTooltipWindow)
        {
            if (_statOpenEnemy == null)
            {
                ClosePopupWindow();
                return;
            }
            else
            {
                SetEnemyStatUIPosition(_statOpenEnemy.transform.position);
            }
        }
    }
    /// <summary>
    /// 적 스텟창을 설정합니다.
    /// 플레이어가 Idle 상태일 때 적 유닛을 클릭하면 해당 적 유닛 옆에 적의 스텟창 UI가 생성됩니다.
    /// </summary>
    /// <param name="enemy"> 클릭한 적 유닛 </param>
    public void SetEnemyStatUI(Enemy enemy)
    {
        if (_isOpenedTooltipWindow) 
        {
            ClosePopupWindow();
            return;
        }
        OpenPopupWindow();
        _statOpenEnemy = enemy;
        SetEnemyStatUIPosition(enemy.transform.position);

        _enemyNameText.GetComponent<TextMeshProUGUI>().text = "WANTED : " + enemy.unitName;
        _enemyInfoUI.GetComponent<EnemyInfoUI>().SetEnemyInfoUI(FieldSystem.unitSystem.GetEnemyData(enemy.dataIndex), 0);
    }
    private void SetEnemyStatUIPosition(Vector3 pos)
    {
        //Select UI Right or Left side
        Player player = FieldSystem.unitSystem.GetPlayer();
        int sign = (int)(pos.x - player.transform.position.x);
        if (sign == 0)
        {
            sign = -1;
        }
        sign = sign / Mathf.Abs(sign);

        Vector3 enemyPositionHeightCorrection = pos;
        enemyPositionHeightCorrection.y += 1.8f;

        Vector2 enemyScreenPos = Camera.main.WorldToScreenPoint(enemyPositionHeightCorrection);
        Vector2 windowSetPos = enemyScreenPos;
        windowSetPos.x += sign * WINDOW_X_POSITION_CORRECTION * UIManager.instance.GetCanvasScale();

        //Set UI Position
        _enemyStatWindow.GetComponent<RectTransform>().position = windowSetPos;

        //Screen Range Correction
        Vector2 enemyScreenLocalPos = _enemyStatWindow.GetComponent<RectTransform>().localPosition;

        Vector2 canvasSize = GetComponent<RectTransform>().sizeDelta;
        Vector2 windowSize = _enemyStatWindow.GetComponent<RectTransform>().sizeDelta;
        float xPosLimit = (canvasSize.x / 2 - windowSize.x / 2) - 5;
        float yPosLimit = (canvasSize.y / 2 - windowSize.y / 2) - 5;

        if (enemyScreenLocalPos.x > xPosLimit) enemyScreenLocalPos.x = xPosLimit;
        if (enemyScreenLocalPos.x < -xPosLimit) enemyScreenLocalPos.x = -xPosLimit;
        if (enemyScreenLocalPos.y > yPosLimit) enemyScreenLocalPos.y = yPosLimit;
        if (enemyScreenLocalPos.y < -yPosLimit) enemyScreenLocalPos.y = -yPosLimit;

        _enemyStatWindow.GetComponent<RectTransform>().localPosition = enemyScreenLocalPos;

        //Alpha setting
        Vector2 windowScreenPos = _enemyStatWindow.GetComponent<RectTransform>().position;
        bool isEnemyInWindowXpos = (windowScreenPos.x - windowSize.x / 2 < enemyScreenPos.x && enemyScreenPos.x < windowScreenPos.x + windowSize.x / 2);
        bool isEnemyInWindowYpos = (windowScreenPos.y - windowSize.y / 2 < enemyScreenPos.y && enemyScreenPos.y < windowScreenPos.y + windowSize.y / 2);
        float alpha = 0;
        if (isEnemyInWindowXpos && isEnemyInWindowYpos)
        {
            alpha = 0.5f;
        }
        else 
        {
            alpha = 1;
        }
        Color c = _enemyStatWindow.GetComponent<Image>().color;
        float threshold = 0.01f;
        if (Mathf.Abs(c.a - alpha) > threshold)
        {
            c.a = Mathf.Lerp(c.a, alpha, Time.deltaTime * 2);
        }
        else
        {
            c.a = alpha;
        }
        _enemyStatWindow.GetComponent<Image>().color = c;
    }

    /// <summary>
    /// 적 스텟창 UI를 닫습니다.
    /// </summary>
    public void CloseEnemyStatUI()
    {
        _enemyStatWindow.SetActive(false);
    }
    private static bool IsMouseOverOnEnemy(out Vector3Int pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isSuccessRaycast = Physics.Raycast(ray, out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Enemy"));
        if (isSuccessRaycast)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            pos = enemy.hexPosition;
            return true;
        }

        pos = Vector3Int.zero;
        return false;
    }
}
