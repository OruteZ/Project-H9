using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombatFinishUI : MonoBehaviour
{
    public GameObject combatFinishCanvas;
    public GameObject gameOverCanvas;

    public void Start()
    {
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        combatFinishCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    public void BackToWorld()
    {
        GameManager.instance.FinishCombat();
    }

    public void BackToMenu()
    {
        #if UNITY_EDITOR

        EditorApplication.isPlaying = false;
        
        #endif
        Application.Quit();
    }

    private void OnCombatFinish(bool isPlayerWin)
    {
        if(isPlayerWin is false) Invoke(nameof(TurnOnDefeatUI), 2f);
        else Invoke(nameof(TurnOnWinUI), 2f);
    }

    private void TurnOnDefeatUI()
    {
        gameOverCanvas.SetActive(true);
    }

    private void TurnOnWinUI()
    {
        combatFinishCanvas.SetActive(true);
    }
}
