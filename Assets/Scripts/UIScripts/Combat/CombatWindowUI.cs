using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.AI;

/// <summary>
/// 전투 씬 입장 시 필요한 UI의 여러 기능을 묶어서 관리하는 클래스
/// 이름이 겹쳐서 CombatWindowUI라고 명명했으나, 명칭 일관성을 생각하면 CombatUI가 맞음.
/// </summary>
public class CombatWindowUI : UISystem
{
    [SerializeField] private GameObject _ResultWindowPrefab;

    /// <summary>
    /// 캐릭터의 행동창 UI의 표시 및 상호작용과 관련된 기능
    /// </summary>
    public CombatActionUI combatActionUI { get; private set; }
    /// <summary>
    /// 적들의 체력바를 표시하는 기능
    /// </summary>
    public EnemyHpUI enemyHpUI { get; private set; }
    /// <summary>
    /// 적들의 스텟을 표시하는 스텟창과 관련된 기능
    /// </summary>
    public EnemyStatUI enemyStatUI { get; private set; }
    /// <summary>
    /// 현재 시작되는 턴 주인에 맞춰서 화면 중앙에 텍스트를 출력하는 기능
    /// </summary>
    public CombatPopupTextUI combatPopupTextUI { get; private set; }
    public TurnOrderUI turnOrderUI { get; private set; }
    public CombatResultUI combatResultUI { get; private set; }
    public BuffUI buffUI { get; private set; }
    public TileObjectUI tileObjectUI { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        var resultWindowInstance = Instantiate(_ResultWindowPrefab, UIManager.instance.HotCanvas.transform);
        resultWindowInstance.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        resultWindowInstance.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        combatActionUI = GetComponent<CombatActionUI>();
        enemyHpUI = GetComponent<EnemyHpUI>();
        enemyStatUI = GetComponent<EnemyStatUI>();
        combatPopupTextUI = GetComponent<CombatPopupTextUI>();
        turnOrderUI = GetComponent<TurnOrderUI>();
        combatResultUI = resultWindowInstance.GetComponent<CombatResultUI>();
        buffUI = GetComponent<BuffUI>();
        tileObjectUI = GetComponent<TileObjectUI>();

        //uiSubsystems.Add(combatActionUI);
        uiSubsystems.Add(enemyHpUI);
        uiSubsystems.Add(enemyStatUI);
        uiSubsystems.Add(combatPopupTextUI);
        //uiSubsystems.Add(turnOrderUI);
        uiSubsystems.Add(combatResultUI);
        uiSubsystems.Add(buffUI);
        uiSubsystems.Add(tileObjectUI);
    }

    public override void ClosePopupWindow()
    {
        combatActionUI.CloseUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        //enemyStatUI.ClosePopupWindow();
    }
}
