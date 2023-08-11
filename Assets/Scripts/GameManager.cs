using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Play,
    Loading
}

public enum GameState
{
    Running,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalHp, currentHp;

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onPlaySceneLoaded;
    public UnityEvent onGameOver;
    public event Action<bool> onGamePaused;

    private Coroutine playSceneSetupProcess;
    private Coroutine playSceneProcess;
    private Coroutine sceneLoadingProcess;

    
    public bool isGameOver { get; private set; }
    public GameState gameState { get; private set; }
    void Awake()
    {
        if(!Instance) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onPlaySceneLoaded = new UnityEvent();
        onGameOver = new UnityEvent();
        onGameOver.AddListener(SaveData);
    }

    public bool LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // 씬 로드 시작
            if (sceneLoadingProcess is not null) StopCoroutine(sceneLoadingProcess);
            sceneLoadingProcess = StartCoroutine(SceneLoadingProcess(nextSceneIndex));
            return true;
        }

        return false;
    }

    public bool LoadScene(SceneType sceneType)
    {
        int nextSceneIndex = (int)sceneType;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // 씬 로드
            if (sceneLoadingProcess is not null) StopCoroutine(sceneLoadingProcess);
            sceneLoadingProcess = StartCoroutine(SceneLoadingProcess(nextSceneIndex));
            return true;
        }

        return false;
    }

    private IEnumerator SceneLoadingProcess(int nextSceneIndex)
    {
       FadeUI.Instance.StartFadeOut();
        yield return new WaitUntil(() => FadeUI.Instance.CurFadeState.Equals(FadeUI.FadeState.Fade));
        LoadingSceneController.LoadScene(nextSceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeUI.Instance.StartFadeIn();
        if (scene.buildIndex == (int)SceneType.Play)
        {
            gameState = GameState.Running;
            onPlaySceneLoaded.Invoke();
            playSceneSetupProcess = StartCoroutine(PlaySceneSetupProcess());
        }
        else if (scene.buildIndex == (int)SceneType.Main)
        {
            gameState = GameState.Running;
            onMainSceneLoaded.Invoke();
        }
    }

    private IEnumerator PlaySceneSetupProcess()
    {
        totalHp = PlayerPrefs.HasKey("PlayerTotalHp") ? PlayerPrefs.GetInt("PlayerTotalHp") : 20;
        currentHp = PlayerPrefs.HasKey("PlayerCurrentHp") ? PlayerPrefs.GetInt("PlayerCurrentHp") : 20;
        yield return null;

        playSceneProcess = StartCoroutine(PlaySceneProcess());
    }

    private IEnumerator PlaySceneProcess()
    {
        while (isGameOver)
        {
            yield return null;
        }
        onGameOver.Invoke();
    }

    private void OnPlayerDeath()
    {
        isGameOver = true;
    }

    public void PauseGame(bool isPaused)
    {
        onGamePaused(isPaused);
        gameState = isPaused ? GameState.Paused : GameState.Running;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("PlayerTotalHp", totalHp);
        PlayerPrefs.SetInt("PlayerCurrentHp", currentHp);
    }

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
