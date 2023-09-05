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
    [SerializeField] private GameObject _turnOrderUIPrefabs;
    [SerializeField] private GameObject _turnOrderUIPrefab;

    private const int TURN_ORDER_UI_LENGTH = 12;

    private void Start()
    {
        TurnOrderUIObjectPooling(30);
    }

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
    public void SetTurnText() 
    {
        int currentTurn = 0;
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            currentTurn = FieldSystem.unitSystem.GetPlayer().currentRound;
        }
        else 
        {
            currentTurn = FieldSystem.turnSystem.turnNumber;
        }
        _turnText.GetComponent<TextMeshProUGUI>().text = "Turn " + currentTurn;
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
    /// 전투 씬에서 처음 실행된 경우 InitTurnOrderUI함수를 통해 UI들의 초기 위치를 설정하고,
    /// 그 이후에 실행될 경우 ChangeTurnOrderUI함수를 통해 기존 UI들의 위치를 이동시켜서 표시합니다.
    /// </summary>
    /// <param name="turnOrder"> 턴 순서 리스트 </param>
    public void SetTurnOrderUI(List<Unit> turnOrder) 
    {
        if (_turnOrderUI.transform.childCount <= 0)
        {
            InitTurnOrderUI(turnOrder);
        }
        else 
        {
            ChangeTurnOrderUI(turnOrder);
        }
    }
    private void AddTurnOrderUI(Unit unit, int order)
    {
        if (_turnOrderUIPrefabs.transform.childCount <= 0)
        {
            TurnOrderUIObjectPooling(15);
        }
        GameObject ui = _turnOrderUIPrefabs.transform.GetChild(0).gameObject;
        ui.transform.SetParent(_turnOrderUI.transform);
        ui.GetComponent<TurnOrderUIElement>().InitTurnOrderUIElement(unit, order);
        ui.SetActive(true);
    }
    private void DeleteTurnOrderUI(GameObject ui)
    {
        ui.transform.SetParent(_turnOrderUIPrefabs.transform);
        ui.SetActive(false);
    }
    private void InitTurnOrderUI(List<Unit> turnOrder)
    {
        int order = 0;
        foreach (Unit unit in turnOrder)
        {
            AddTurnOrderUI(unit, order++);
        }
    }
    private void ChangeTurnOrderUI(List<Unit> turnOrder)
    {
        //현재 턴순서의 사본
        List<GameObject> currentTurnOrderUI = new List<GameObject>();
        for (int i = 0; i < _turnOrderUI.transform.childCount; i++)
        {
            currentTurnOrderUI.Add(_turnOrderUI.transform.GetChild(i).gameObject);
        }

        //currentTurnOrderUI의 해당 요소를 체크했는지 저장
        bool[] check = new bool[currentTurnOrderUI.Count];
        for (int i = 0; i < check.Length; i++) check[i] = false;

        //변경될 순서의 요소들을 현재 currentTurnOrderUI의 인덱스로 표현해 순서대로 저장
        List<int> changeOrder = new List<int>();

        //입력된 turnOrder에 맞게 changeOrder를 생성.
        //turnOrder순서대로 currentTurnOrderUI에서 해당 요소를 검색한 후,
        //check를 true로 지정한 후 해당 요소의 인덱스를 changeOrder에 저장.
        //만약 turnOrder의 다음 요소가 currentTurnOrderUI에 존재하지 않는다면, currentTurnOrderUI가 아니라 실제 _turnOrderUI에 새로 해당 요소를 생성.
        foreach (Unit unit in turnOrder) 
        {
            bool isFound = false;
            for (int i = 1; i < currentTurnOrderUI.Count; i++) 
            {
                bool isSameTypeObject = (currentTurnOrderUI[i].GetComponent<TurnOrderUIElement>()._unit.GetType() == unit.GetType());
                bool isAlreadyCheck = check[i];
                if (isSameTypeObject && !isAlreadyCheck) 
                {
                    changeOrder.Add(i);
                    check[i] = true;
                    isFound = true;
                    break;
                }
            }
            if (!isFound)
            {
                AddTurnOrderUI(unit, TURN_ORDER_UI_LENGTH);
                changeOrder.Add(_turnOrderUI.transform.childCount - 1);
            }
        }

        //기존에 없던 요소를 생성한 _turnOrderUI에 따라 currentTurnOrderUI를 다시 갱신
        currentTurnOrderUI = new List<GameObject>();
        for (int i = 0; i < _turnOrderUI.transform.childCount; i++)
        {
            currentTurnOrderUI.Add(_turnOrderUI.transform.GetChild(i).gameObject);
        }
        //표시 순서를 지키기 위해 현재 표시중인 UI들을 임시로 전부 오브젝트 풀링 대기 장소로 이동.(원본 순서는 currentTurnOrderUI가 기억)
        while (_turnOrderUI.transform.childCount > 0)
        {
            _turnOrderUI.transform.GetChild(0).SetParent(_turnOrderUIPrefabs.transform);
        }

        //생성한 changeOrder에 따라 currentTurnOrderUI에서 순서대로 요소를 선택해 _turnOrderUI로 다시 이동하고 changeOrder함수를 실행해 UI 위치를 이동하도록 명령
        for (int i = 0; i < changeOrder.Count; i++)
        {
            Debug.Log(currentTurnOrderUI[changeOrder[i]].GetComponent<TurnOrderUIElement>()._unit + "index: "+ changeOrder[i]);
            currentTurnOrderUI[changeOrder[i]].transform.SetParent(_turnOrderUI.transform);
            currentTurnOrderUI[changeOrder[i]].GetComponent<TurnOrderUIElement>().ChangeOrder(i);
        }

        //입력된 turnOrder에 없던 요소들은 그대로 비활성화.
        for (int i = 0; i < check.Length; i++)
        {
            if (check[i] == false)
            {
                DeleteTurnOrderUI(currentTurnOrderUI[i].gameObject);
            }
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

    private void TurnOrderUIObjectPooling(int length)
    {
        for (int i = 0; i < length; i++)
        {
            GameObject ui = Instantiate(_turnOrderUIPrefab, Vector3.zero, Quaternion.identity, _turnOrderUIPrefabs.transform);
            ui.SetActive(false);
        }
    }
}
