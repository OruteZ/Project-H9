using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Generic.Singleton<LoadingManager>
{
    private new void Awake()
    {
        base.Awake();
        
        canvas = GetComponentInChildren<Canvas>(includeInactive:true);
        progress = GetComponentInChildren<Slider>(includeInactive:true);
    }

    private void Start()
    {
        canvas.enabled = false;
    }

    public Canvas canvas;
    public Slider progress;

    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        SceneManager.LoadScene("LoadingScene");
        
        yield return null;
        canvas.enabled = true;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;

            if (progress.value < 1f)
            {
                
                progress.value = operation.progress;
            }

            operation.allowSceneActivation = true;
        }
        UIManager.instance.ChangeScene(SceneNameToGameState(sceneName));
        canvas.enabled = false;
    }

    /// <summary>
    /// 씬의 이름을 받으면 그에 대응하는 GameState로 변환하여 반환해줍니다.
    /// </summary>
    /// <param name="sceneName"> 씬 이름 </param>
    /// <returns> 입력한 씬 이름에 대응하는 GameState </returns>
    public GameState SceneNameToGameState(string sceneName)
    {
        switch (sceneName)
        {
            case "WorldScene":
            case "UITestScene":
                {
                    return GameState.World;
                }
            case "CombatScene":
                {
                    return GameState.Combat;
                }
        }

        Debug.LogError(sceneName + "이라는 Scene을 찾을 수 없습니다.");
        return GameState.World;
    }
    /// <summary>
    /// GameState를 입력하면 그에 대응하는 씬의 이름을 반환합니다.
    /// </summary>
    /// <param name="gameState"> GameState </param>
    /// <returns> 입력한 GameState에 대응하는 씬 이름 </returns>
    public string GameStateToSceneName(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.World:
                {
                    return "WorldScene";
                }
            case GameState.Combat:
                {
                    return "CombatScene";
                }
        }
        return null;
    }
}
