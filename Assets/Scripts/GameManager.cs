using System.Collections;
using System.Collections.Generic;
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

    public GameState gameState { get; private set; }
    public bool isGameOver { get; private set; }
    public int totalHp, currentHp;

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onPlaySceneLoaded;
    public UnityEvent onGameOver;

    private Coroutine playSceneSetupProcess;

    void Awake()
    {
        if(!Instance) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onPlaySceneLoaded = new UnityEvent();
        onGameOver = new UnityEvent();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)SceneType.Play)
        {
            gameState = GameState.Running;
            onMainSceneLoaded.Invoke();
        }
        else if (scene.buildIndex == (int)SceneType.Main)
        {
            gameState = GameState.Running;
            onPlaySceneLoaded.Invoke();
            playSceneSetupProcess = StartCoroutine(PlaySceneSetupProcess());
        }
    }

    private IEnumerator PlaySceneSetupProcess()
    {
        totalHp = PlayerPrefs.HasKey("PlayerTotalHp") ? PlayerPrefs.GetInt("PlayerTotalHp") : 20;
        currentHp = PlayerPrefs.HasKey("PlayerCurrentHp") ? PlayerPrefs.GetInt("PlayerCurrentHp") : 20;
        yield return null;
    }
}
