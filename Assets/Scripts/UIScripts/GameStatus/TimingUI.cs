using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 진행한 턴 수를 카운트하고, 전투 중에는 다음에 오는 턴의 주인이 누구인지 순서대로 표시하는 기능을 구현한 클래스
/// </summary>
public class TimingUI : UISystem
{
    [SerializeField] private GameObject _turnText;
    [SerializeField] private GameObject _turnOrderUI;
    private readonly Vector3 TURN_ORDER_UI_INIT_POSITION = new Vector3(0, 0, 0);
    private const int TURN_ORDER_UI_INTERVAL = 10;

    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    /// <summary>
    /// 화면 우측 상단에 현재 턴 수를 표시한다.
    /// </summary>
    /// <param name="currentTurn"> 현재 턴 수 </param>
    public void SetTurnText(int currentTurn) 
    {
        _turnText.GetComponent<TextMeshProUGUI>().text = currentTurn + "턴";
    }
    /// <summary>
    /// 턴 순서 UI를 키거나 끕니다.
    /// 전투 씬일 때 켜지고 아닐 때 꺼집니다.
    /// </summary>
    /// <param name="isOn"> UI 활성화 상태 </param>
    public void SetTurnOrderUIState(bool isOn) 
    {
        _turnOrderUI.SetActive(isOn);
    }
    /// <summary>
    /// 턴 순서 UI를 설정합니다.
    /// turnSystem에서 계산한 턴 순서를 입력받아 UI에 표시합니다.
    /// </summary>
    /// <param name="turnOrder"> 턴 순서 리스트 </param>
    public void SetTurnOrderUI(List<Unit> turnOrder) 
    {
        int index = 0;
        foreach(Unit unit in turnOrder)
        {
            //unit의 index를 참조하여 적절한 초상화 sprite를 불러와야 하는데... 어디서 찾아와야 할지 아직 모름.
            if (unit is Player)
            {
                _turnOrderUI.transform.GetChild(index).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                _turnOrderUI.transform.GetChild(index).GetComponent<Image>().color = new Color(.5f, .5f, 0.5f, 1);
            }
            index++;
        }
    }

    /// <summary>
    /// 턴 종료 버튼을 클릭할 시 실행됩니다.
    /// turnSystem에 플레이어의 턴을 종료하라고 명령을 보냅니다.
    /// </summary>
    public void OnClickEndTurnButton() 
    {
        if (FieldSystem.turnSystem.turnOwner is Player)
        {
            FieldSystem.turnSystem.EndTurn();
        }
    }
}
