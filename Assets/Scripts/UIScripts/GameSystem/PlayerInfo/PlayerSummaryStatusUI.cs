using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어의 스텟 중 체력, 집중력, 액션 포인트를 표시하는 UI의 기능을 수행하는 클래스
/// </summary>
public class PlayerSummaryStatusUI : UIElement
{
    private Unit _player;

    [SerializeField] private GameObject _healthPointUI;
    [SerializeField] private GameObject _actionPointUI;
    [SerializeField] private GameObject _magazineUI;
    [SerializeField] private GameObject _concentrationUI;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentStatusUI();
    }
    private void Update()
    {
        //for test
        //SetCurrentStatusUI();
        //유닛의 스텟 변화 시점을 감지할 방법이...? Unit의 stat변수는 protected이고... UnityEvent는 마땅한 것이 없다.
        //stat 변수의 set을 이용해서 해도 되긴 하는데 내가 구현한 게 아니라서 조심스럽다. 향후 리펙토링 시 고려.
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
    /// 상태 UI 정보를 설정합니다.
    /// </summary>
    public void SetCurrentStatusUI()
    {
        Unit _player = FieldSystem.unitSystem.GetPlayer();
        if (_player is null) return;

        _healthPointUI.GetComponent<PlayerHpUI>().SetHpUI();
        _actionPointUI.GetComponent<PlayerApUI>().SetApUI();
        _magazineUI.GetComponent<PlayerMagazineUI>().SetMagazineUI(false);
        _concentrationUI.GetComponent<PlayerConcentrationUI>().SetConcentrationUI();
    }
}
