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
        canvas = GetComponentInChildren<Canvas>(includeInactive:true);
        progress = GetComponentInChildren<Slider>(includeInactive:true);
        
        //if one of two is null, remove this
        if (canvas is null || progress is null)
        {
            Debug.LogError("LoadingManager의 Canvas 또는 Slider가 비어있습니다.");
            Destroy(this);
        }
        base.Awake();
    }

    private void Start()
    {
        canvas.enabled = false;
        progress.gameObject.SetActive(false);
        isLoadingNow = false;
    }

    public Canvas canvas;
    public Slider progress;

    public void LoadingScene(string sceneName, Action callback = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, callback));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName, Action callback = null)
    {
        isLoadingNow = true;
        canvas.enabled = true;
        yield return StartCoroutine(FadeOut(fadeDuration));
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        progress.gameObject.SetActive(true);
        while (operation.progress < 0.9f)
        {
            yield return null;
            progress.value = operation.progress;
        }
        while (progress.value < 1f)
        {
            yield return new WaitForSeconds(0.01f);
            progress.value += 0.01f;
        }
        
        operation.allowSceneActivation = true;
        progress.gameObject.SetActive(false);
        
        UIManager.instance.ChangeScene(SceneNameToGameState(sceneName));

        yield return StartCoroutine(FadeIn(fadeDuration));
        canvas.enabled = false;

        isLoadingNow = false; 
        
        callback?.Invoke();
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
                    return GameState.WORLD;
                }
            case "CombatScene":
                {
                    return GameState.COMBAT;
                }
            case "TitleScene":
                {
                    return GameState.NONE;
                }
        }

        Debug.LogError(sceneName + "이라는 Scene을 찾을 수 없습니다.");
        return GameState.WORLD;
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
            GameState.WORLD => "WorldScene",
            GameState.COMBAT => "CombatScene",
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
