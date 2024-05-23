using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메뉴 버튼을 눌렀을 때 표시할 각종 UI를 관리하는 클래스
/// </summary>
public class PauseMenuUI : UISystem
{
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
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        #else
        Application.Quit();
        
        #endif
    }
}
