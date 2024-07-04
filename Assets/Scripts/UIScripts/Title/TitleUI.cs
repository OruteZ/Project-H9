using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject _optionWindow;
    [SerializeField] private LoadUI _loadPanel;
    [SerializeField] private GameObject _loadingManager;

    private bool _isOpenOption = false;
    private bool _isStarted = false;
    // Start is called before the first frame update
    void Awake()
    {
        _loadingManager.SetActive(true);
    }

    public void OnClickStartBtn()
    {
        if (!_isStarted) 
        {
            DataLoader.New();
            _isStarted = true;
            LoadingManager.instance.LoadingScene("WorldScene");
        }
    }
    public void OnClickLoadPanel()
    {
        if (!_isStarted)
        {
            _loadPanel.OpenUI();
        }
    }
    public void OnClickLoadSlot(in UserData userData)
    {
        if (!_isStarted) 
        {
            DataLoader.Stack(userData);
            _isStarted = true;
            LoadingManager.instance.LoadingScene("WorldScene");
        }
    }
    public void OnClickOptionBtn()
    {
        _isOpenOption = !_isOpenOption;
        _optionWindow.SetActive(_isOpenOption);
    }
    public void OnClickExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
