using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 사용법:
///  1. Debug Canvas 하위의 Debug Window에 빈 오브젝트를 생성한 후 해당 디버깅을 위한 코드B를 하나 생성해서 삽입한다.
///  2. 빈 오브젝트 하위에 디버깅에 필요한 UI 요소들을 삽입한다.
///  3. 삽입한 UI 요소를 이용하여 해당 디버깅을 위한 기능을 그 코드B에 구현한다.
///  4. 현재 코드A의 SetDebugUI()에 필요한 매개변수를 넣어 오버로드하여 새로운 코드A를 호출하게 한다.
///  5. 새로 만든 SetDebugUI()를 Unit, TurnSystem 등의 다른 코드C의 적절한 위치에서 호출하게 한다.
///  6. 잘 실행되는지 확인한다.
/// <summary>
/// 개발 중에 인스펙터창으로 보기 불편한 여러 요소들을 UI로 표시하는 클래스
/// </summary>
public class DebugUI : UISystem
{
    [SerializeField] private GameObject _debugWindow;

    public bool isOpenDebugWindow;
    // Start is called before the first frame update
    void Start()
    {
        isOpenDebugWindow = false;
        GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (isOpenDebugWindow)
            {
                CloseUI();
                GetComponent<Canvas>().enabled = false;
            }
            else
            {
                GetComponent<Canvas>().enabled = true;
                OpenUI();
            }
            isOpenDebugWindow = !isOpenDebugWindow;
        }
    }
    /// <summary>
    /// 명중률 확인을 위한 디버깅 UI를 갱신한다.
    /// 현재는 리볼버의 GetFinalHitRate()함수 실행시 작동한다.
    /// 다른 총을 테스트하고 싶으면 다른 총의 코드에서도 실행해야 작동할 것.
    /// </summary>
    /// <param name="hitRate"></param>
    /// <param name="unit"></param>
    /// <param name="target"></param>
    /// <param name="dist"></param>
    /// <param name="wRange"></param>
    /// <param name="addRange"></param>
    /// <param name="penalty"></param>
    public void SetDebugUI(float hitRate, Unit unit, IDamageable target, float dist, float wRange, float addRange, float penalty) 
    {
        UISystem ui = FindActiveDebugUI();
        if (ui is HitRateDebugWindow)
        {
            ((HitRateDebugWindow)ui).SetText(hitRate, unit, target, dist, wRange, addRange, penalty);
        }
        //else if (ui is DebugWindow) 
        //{
        //    ((DebugWindow)ui).SetUI();
        //}
    }

    /// <summary>
    /// 예시 코드 함수, 실행해도 아무 일도 일어나지 않음.
    /// </summary>
    public void SetDebugUI()
    {
        UISystem ui = FindActiveDebugUI();
        //if (ui is DebugWindow) 
        //{
        //    ((DebugWindow)ui).SetUI();
        //}
    }
    public override void OpenUI()
    {
        base.OpenUI();
        UISystem ui = FindActiveDebugUI();
        if (ui is null) return;
        ui.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        UISystem ui = FindActiveDebugUI();
        if (ui is null) return;
        ui.CloseUI();
    }
    private UISystem FindActiveDebugUI()
    {
        for (int i = 0; i < _debugWindow.transform.childCount; i++)
        {
            if (_debugWindow.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                return _debugWindow.transform.GetChild(i).GetComponent<UISystem>();
            }
        }
        return null;
    }
}
