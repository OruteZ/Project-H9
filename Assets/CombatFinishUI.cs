using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatFinishUI : MonoBehaviour
{
    public GameObject combatFinishCanvas;
    public GameObject gameOverCanvas;

    public void Start()
    {
        //FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        combatFinishCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    public void BackToWorld()
    {
        GameManager.instance.FinishCombat();
    }

    public void BackToMenu()
    {
        // Find all GameObjects with DontDestroyOnLoad
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        
        SceneManager.LoadScene($"TitleScene");
    }

    private void OnCombatFinish(bool isPlayerWin)
    {
        Invoke(isPlayerWin is false ? nameof(TurnOnDefeatUI) : nameof(TurnOnWinUI), 2f);
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
