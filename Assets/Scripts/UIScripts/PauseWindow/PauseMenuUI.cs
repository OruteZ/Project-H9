using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메뉴 버튼을 눌렀을 때 표시할 각종 UI를 관리하는 클래스
/// </summary>
public class PauseMenuUI : UISystem
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnResumeBtnClick()
    {
        UIManager.instance.SetUILayer(1);
    }
}
