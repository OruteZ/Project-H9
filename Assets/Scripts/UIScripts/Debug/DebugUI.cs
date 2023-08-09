using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void SetDebugUI(float hitRate, Unit unit, Unit target, float dist, float wRange, float addRange, float penalty) 
    {
        UISystem ui = FindActiveDebugUI();
        if (ui is not HitRateDebugWindow) return;
        ((HitRateDebugWindow)ui).SetText(hitRate, unit, target, dist, wRange, addRange, penalty);
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
