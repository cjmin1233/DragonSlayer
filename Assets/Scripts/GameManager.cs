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
    public GameState gameState { get; private set; }
    public bool isGameOver { get; private set; }
    public int totalHp, currentHp;

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onPlaySceneLoaded;
    public UnityEvent onBossSceneLoaded;
    public UnityEvent onGameOver;
    public event Action<bool> onGamePaused;

    private Coroutine playSceneSetupProcess;
    private Coroutine playSceneProcess;
    private Coroutine bossSceneSetupProcess;
    private Coroutine bossSceneProcess;

    void Awake()
    {
        if(!Instance) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        generatedRooms = new List<GeneratedRoomInfo>();
        aliveEnemies = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onPlaySceneLoaded = new UnityEvent();
        onBossSceneLoaded = new UnityEvent();
        onGameOver = new UnityEvent();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
        MapVector2.instance.GenerateDungeon();
        Minimap.Instance.MinimapCreate();
        EnemySpawner.Instance.SelectEnemySpawner();

        totalHp = PlayerPrefs.HasKey("PlayerTotalHp") ? PlayerPrefs.GetInt("PlayerTotalHp") : 20;
        currentHp = PlayerPrefs.HasKey("PlayerCurrentHp") ? PlayerPrefs.GetInt("PlayerCurrentHp") : 20;
        yield return null;

        playSceneProcess = StartCoroutine(PlaySceneProcess());
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
        yield return null;
    }

    private IEnumerator BossSceneProcess()
    {
        yield return null;
    }

    private void OnPlayerDeath()
    {
        isGameOver = true;
    }

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

    public void QuitGame() => Application.Quit();
}
