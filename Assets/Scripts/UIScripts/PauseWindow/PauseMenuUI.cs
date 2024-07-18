using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메뉴 버튼을 눌렀을 때 표시할 각종 UI를 관리하는 클래스
/// </summary>
public class PauseMenuUI : UISystem
{
    public OptionUI optionUI { get; private set; }
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private LoadUI _loadPanel;

    private void Awake()
    {
        optionUI = GetComponent<OptionUI>();

        //uiSubsystems.Add(optionUI);
    }
    private void Start()
    {
        _menuPanel.transform.Find("PauseMenu Text").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2100];
        _menuPanel.transform.Find("Buttons/Resume Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2101];
        _menuPanel.transform.Find("Buttons/SaveOpen Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2102];
        _menuPanel.transform.Find("Buttons/Option Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2103];
        _menuPanel.transform.Find("Buttons/MainMenu Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2104];
        _menuPanel.transform.Find("Buttons/Exit Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[2105];
    }
    public void OnResumeBtnClick()
    {
        UIManager.instance.SetUILayer(1);
    }
    
    public void BackToTitle()
    {
        //Find all GameObjects with DontDestroyOnLoad
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        
        SceneManager.LoadScene($"TitleScene");
    }
    public void OnOptionBtnClick() 
    {
        optionUI.OpenUI();
    }
    public override void CloseUI()
    {
        optionUI.CloseUI();
        _loadPanel.CloseUI();
        if (optionUI.isOpened) base.CloseUI();
    }

    public void OnOpenLoadUIClick()
    {
        _loadPanel.OpenUI();
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        #else
        Application.Quit();
        
        #endif
    }
}
