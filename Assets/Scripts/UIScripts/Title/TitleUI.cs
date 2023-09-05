using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject _optionWindow;

    private bool _isOpenOption = false;
    private bool _isStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStartBtn()
    {
        if (!_isStarted) 
        {
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
