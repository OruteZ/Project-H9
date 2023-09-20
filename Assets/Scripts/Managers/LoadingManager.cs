using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Generic.Singleton<LoadingManager>
{
    public bool isLoadingNow;
    [SerializeField] private float fadeDuration;
    private new void Awake()
    {
        base.Awake();
        
        canvas = GetComponentInChildren<Canvas>(includeInactive:true);
        progress = GetComponentInChildren<Slider>(includeInactive:true);
    }

    private void Start()
    {
        canvas.enabled = false;
        progress.gameObject.SetActive(false);
        isLoadingNow = false;
    }

    public Canvas canvas;
    public Slider progress;

    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isLoadingNow = true;
        canvas.enabled = true;
        yield return StartCoroutine(FadeOut(fadeDuration));
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progress.gameObject.SetActive(true);
            yield return null;

            if (progress.value < 1f)
            {
                
                progress.value = operation.progress;
            }
            operation.allowSceneActivation = true;
            progress.gameObject.SetActive(false);
        }
        
        UIManager.instance.ChangeScene(SceneNameToGameState(sceneName));
        
        yield return StartCoroutine(FadeIn(fadeDuration));
        canvas.enabled = false;

        isLoadingNow = false; 
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
        return gameState switch
        {
            GameState.World => "WorldScene",
            GameState.Combat => "CombatScene",
            _ => null
        };
    }

    private IEnumerator FadeIn(float duration)
    {
        var image = canvas.GetComponentInChildren<Image>();

        float reciprocalDuration = 1 / duration;
        float t = 0;
        while ((t += Time.deltaTime) < duration)
        {
            var percentage = t * reciprocalDuration;
            image.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, percentage));
            yield return null;
        }
        image.color = Color.black;
    }

    private IEnumerator FadeOut(float duration)
    {
        var image = canvas.GetComponentInChildren<Image>();

        float reciprocalDuration = 1 / duration;
        float t = 0;
        while ((t += Time.deltaTime) < duration)
        {
            var percentage = t * reciprocalDuration;
            image.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, percentage));
            yield return null;
        }
        image.color = Color.black;
    }
}
