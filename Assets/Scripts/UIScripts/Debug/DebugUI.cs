using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ����:
///  1. Debug Canvas ������ Debug Window�� �� ������Ʈ�� ������ �� �ش� ������� ���� �ڵ�B�� �ϳ� �����ؼ� �����Ѵ�.
///  2. �� ������Ʈ ������ ����뿡 �ʿ��� UI ��ҵ��� �����Ѵ�.
///  3. ������ UI ��Ҹ� �̿��Ͽ� �ش� ������� ���� ����� �� �ڵ�B�� �����Ѵ�.
///  4. ���� �ڵ�A�� SetDebugUI()�� �ʿ��� �Ű������� �־� �����ε��Ͽ� ���ο� �ڵ�A�� ȣ���ϰ� �Ѵ�.
///  5. ���� ���� SetDebugUI()�� Unit, TurnSystem ���� �ٸ� �ڵ�C�� ������ ��ġ���� ȣ���ϰ� �Ѵ�.
///  6. �� ����Ǵ��� Ȯ���Ѵ�.
/// <summary>
/// ���� �߿� �ν�����â���� ���� ������ ���� ��ҵ��� UI�� ǥ���ϴ� Ŭ����
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
    /// ���߷� Ȯ���� ���� ����� UI�� �����Ѵ�.
    /// ����� �������� GetFinalHitRate()�Լ� ����� �۵��Ѵ�.
    /// �ٸ� ���� �׽�Ʈ�ϰ� ������ �ٸ� ���� �ڵ忡���� �����ؾ� �۵��� ��.
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
    /// ���� �ڵ� �Լ�, �����ص� �ƹ� �ϵ� �Ͼ�� ����.
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
