using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject PauseMenu;

    private FadeUI fadePanel;
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this)) Destroy(gameObject);

        eventSystem.SetActive(true);
        DontDestroyOnLoad(gameObject);
        
        // add listener
        GameManager.Instance.onMainSceneLoaded.AddListener(MainSceneSetup);
        GameManager.Instance.onPlaySceneLoaded.AddListener(PlaySceneSetup);
        // GameManager.Instance.onMainSceneLoaded.AddListener(GameOverSetup);
        //
        //
        fadePanel = GetComponentInChildren<FadeUI>();
    }

    private void MainSceneSetup()
    {
        PlayPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    private void PlaySceneSetup()
    {
        MainPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        PlayPanel.SetActive(true);
    }

    private void GameOverSetup()
    {
        MainPanel.SetActive(false);
        PlayPanel.SetActive(false);
        GameOverPanel.SetActive(true);
        
        //
    }


    public void Attempt2LoadNextScene()
    {
        if (GameManager.Instance.LoadNextScene()) print("next scene load complete");
        else print("next scene load failed");
    }

    public void Attempt2LoadMainScene()
    {
        if (GameManager.Instance.LoadScene(SceneType.Main)) print("main scene load complete");
        else print("main scene load failed");
    }

    public void Attempt2QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
