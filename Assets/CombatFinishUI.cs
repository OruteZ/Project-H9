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
        FieldSystem.unitSystem.onCombatFinish.AddListener(() => Invoke(nameof(OnCombatFinish), 2f));
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

    private void OnCombatFinish()
    {
        if(FieldSystem.unitSystem.GetPlayer().GetStat().curHp <= 0) gameOverCanvas.SetActive(true);
        else combatFinishCanvas.SetActive(true);
    }
}
