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

    [SerializeField] private TMP_Text _hpTitleText;
    [SerializeField] private TMP_Text _concentrationTitleText;
    [SerializeField] private TMP_Text _apTitleText;

    [HideInInspector] public int expectedHpUsage = 0;
    [HideInInspector] public int expectedApUsage = 0;
    public int expectedMagUsage = 0;
    [HideInInspector] public int expectedConcenUsage = 0;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.onSceneChanged.AddListener(StageStart);
        FieldSystem.onStageAwake.AddListener(StageStart);
        
        
        InitExpectedValues();
        SetCurrentStatusUI();
    }

    void StageStart()
    {
        InitExpectedValues();
        FieldSystem.turnSystem.onTurnChanged.AddListener(InitExpectedValues);
        FieldSystem.unitSystem.GetPlayer().onActionStart.AddListener((a, p) => { InitExpectedValues(); });
    }
    
    public void InitExpectedValues() 
    {
        expectedHpUsage = 0;
        expectedApUsage = 0;
        expectedMagUsage = 0;
        expectedConcenUsage = 0;
        SetCurrentStatusUI();
    }

    /// <summary>
    /// 상태 UI 정보를 설정합니다.
    /// </summary>
    public void SetCurrentStatusUI()
    {
        Unit _player = FieldSystem.unitSystem.GetPlayer();
        if (_player is null) return;

        _hpTitleText.text = UIManager.instance.UILocalization[26];
        _concentrationTitleText.text = UIManager.instance.UILocalization[29];
        _apTitleText.text = UIManager.instance.UILocalization[27];

        _healthPointUI.GetComponent<PlayerHpUI>().SetHpUI();
        _actionPointUI.GetComponent<PlayerApUI>().SetApUI();
        _magazineUI.GetComponent<PlayerMagazineUI>().SetMagazineUI(false);
        _concentrationUI.GetComponent<PlayerConcentrationUI>().SetConcentrationUI();
    }
}
