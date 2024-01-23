using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class CombatResultUI : UISystem
{
    [SerializeField] private GameObject _ResultWindow;
    [SerializeField] private GameObject _WinOrLoseText;
    [SerializeField] private GameObject _ResultText;
    [SerializeField] private GameObject _CloseButton;

    private bool isPlayerWin;
    private int earnedExp;

    public void Start()
    {
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        UIManager.instance.onSceneChanged.AddListener(CloseCombatResultUI);
        _ResultWindow.SetActive(false);
    }

    public void OnClickResultWindowCloseButton()
    {
        if (isPlayerWin)
        {
            BackToWorld();
        }
        else 
        {
            BackToMenu();
        }
    }
    private void BackToWorld()
    {
        GameManager.instance.FinishCombat();
    }

    private void BackToMenu()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnCombatFinish(bool isPlayerWin)
    {
        this.isPlayerWin = isPlayerWin;
        Invoke(nameof(SetCombatResultUI), 2f);
    }

    private void SetCombatResultUI()
    {
        _ResultWindow.SetActive(true);
        if (isPlayerWin)
        {
            _WinOrLoseText.GetComponent<TextMeshProUGUI>().text = "You Win!";
        }
        else
        {
            _WinOrLoseText.GetComponent<TextMeshProUGUI>().text = "Game Over";
        }
        _ResultText.GetComponent<TextMeshProUGUI>().text = "Earned EXP: " + earnedExp;
        UIManager.instance.gameSystemUI.playerInfoUI.expUI.GetComponent<PlayerExpUI>().SetPlayerExpUI(GameManager.instance.curExp + earnedExp);
        earnedExp = 0;
    }
    private void CloseCombatResultUI()
    {
        _ResultWindow.SetActive(false);
    }
    public void SetExpInformation(int eExp) 
    {
        earnedExp = eExp;
    }
}
