using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Play,
    Boss,
    Loading,
}

public enum GameState
{
    Running,
    Paused
}

public struct GeneratedRoomInfo
{
    public Vector3 roomPosition;
    public bool isRoomClear;

    public GeneratedRoomInfo(Vector3 roomPosition, bool isRoomClear = false)
    {
        this.roomPosition = roomPosition;
        this.isRoomClear = isRoomClear;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GeneratedRoomInfo> generatedRooms {  get; private set; }
    public int playerRoomIndex;
    public int aliveEnemies;
    public int totalHp, currentHp;

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onPlaySceneLoaded;
    public UnityEvent onBossSceneLoaded;
    public UnityEvent onGameOver;
    public event Action<bool> onGamePaused = _ => { };

    private Coroutine playSceneSetupProcess;
    private Coroutine playSceneProcess;
    private Coroutine bossSceneSetupProcess;
    private Coroutine bossSceneProcess;
    private Coroutine sceneLoadingProcess;
    private Coroutine stageMovingProcess;

    public bool isGameOver { get; private set; }
    public GameState gameState { get; private set; }
    private void Start()
    {
        if (!Instance) Instance = this;
        else if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        generatedRooms = new List<GeneratedRoomInfo>();
        aliveEnemies = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onPlaySceneLoaded = new UnityEvent();
        onBossSceneLoaded = new UnityEvent();
        onGameOver = new UnityEvent();
        // onGameOver.AddListener(ShowGameOverPanel);
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
            if (sceneType.Equals(SceneType.Main)) ClearSingleton();
            // 씬 로드
            if (sceneLoadingProcess is not null) StopCoroutine(sceneLoadingProcess);
            sceneLoadingProcess = StartCoroutine(SceneLoadingProcess(nextSceneIndex));
            return true;
        }

        return false;
    }

    private IEnumerator SceneLoadingProcess(int nextSceneIndex)
    {
        UiManager.Instance.FadeOut();
        yield return new WaitUntil(() => UiManager.Instance.FadeState.Equals(FadeUI.FadeState.Fade));
        UiManager.Instance.LoadingSceneSetup();
        LoadingSceneController.LoadScene(nextSceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!UiManager.Instance.FadeState.Equals(FadeUI.FadeState.None)) UiManager.Instance.FadeIn();
        
        if (scene.buildIndex == (int)SceneType.Main)
        {
            gameState = GameState.Running;
            onMainSceneLoaded.Invoke();
        }
        else if (scene.buildIndex == (int)SceneType.Play)
        {
            gameState = GameState.Running;
            onPlaySceneLoaded.Invoke();
            playSceneSetupProcess = StartCoroutine(PlaySceneSetupProcess());
        }
        else if (scene.buildIndex == (int)SceneType.Boss)
        {
            gameState = GameState.Running;
            onBossSceneLoaded.Invoke();
            bossSceneSetupProcess = StartCoroutine(BossSceneSetupProcess());
        }
    }

    private IEnumerator PlaySceneSetupProcess()
    {
        MapVector2.Instance.GenerateDungeon();

        totalHp = PlayerPrefs.HasKey("PlayerTotalHp") ? PlayerPrefs.GetInt("PlayerTotalHp") : 20;
        currentHp = PlayerPrefs.HasKey("PlayerCurrentHp") ? PlayerPrefs.GetInt("PlayerCurrentHp") : 20;
        yield return null;

        playSceneProcess = StartCoroutine(PlaySceneProcess());
        PlayerHealth.Instance.OnDeath += OnPlayerDeath;
    }

    private IEnumerator PlaySceneProcess()
    {
        while (!isGameOver)
        {
            yield return null;
        }
        onGameOver.Invoke();
    }

    private IEnumerator BossSceneSetupProcess()
    {
        // PlayerHealth.Instance.transform.position = Vector3.zero;
        PlayerHealth.Instance.BossSceneEnter();
        yield break;
    }

    private IEnumerator BossSceneProcess()
    {
        yield return null;
    }

    private void OnPlayerDeath()
    {
        isGameOver = true;
        print("Gamemanager on player death");
    }

    // private void ShowGameOverPanel()
    // {
    //     UiManager.Instance.GameOverPanel.gameObject.SetActive(true);
    // }

    public bool IsRoomCleared()
    {
        if (aliveEnemies == 0)
        {
            GeneratedRoomInfo generatedRoomInfo = new GeneratedRoomInfo(generatedRooms[playerRoomIndex].roomPosition, true);
            generatedRooms[playerRoomIndex] = generatedRoomInfo;
            return true;
        }
        return false;
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

    private void ClearSingleton()
    {
        isGameOver = false;
        aliveEnemies = 0;
        PlayerHealth.Instance.OnDeath -= OnPlayerDeath;
        if(PlayerHealth.Instance is not null) Destroy(PlayerHealth.Instance.gameObject);
        if(MapVector2.Instance is not null) Destroy(MapVector2.Instance.gameObject);
        if(MapGenerator.Instance is not null) Destroy(MapGenerator.Instance.gameObject);
    }

    public void MoveNextStage()
    {
        if (!UiManager.Instance.FadeState.Equals(FadeUI.FadeState.None)) return;
        if (stageMovingProcess is not null) StopCoroutine(stageMovingProcess);
        stageMovingProcess = StartCoroutine(StageMovingProcess());
        print("stage move");
    }

    private IEnumerator StageMovingProcess()
    {
        UiManager.Instance.FadeOut();
        yield return new WaitUntil(() => UiManager.Instance.FadeState.Equals(FadeUI.FadeState.Fade));
        generatedRooms.Clear();
        EnemySpawner.Instance.MapRecordClear();

        MapGenerator.Instance.epicSize = 0;
        MapVector2.Instance.Stage++;
        PlayerHealth.Instance.transform.position = Vector3.zero;
        MapVector2.Instance.GenerateDungeon();
        MinimapCameraFollow.Instance.FollowMinimap();
        UiManager.Instance.FadeIn();
    }
}
