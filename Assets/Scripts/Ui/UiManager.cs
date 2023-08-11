using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private FadeUI fadePanel;

    [SerializeField] private AudioMixer audioMixer;

    public FadeUI.FadeState FadeState
    {
        get
        {
            return fadePanel.CurFadeState;
        }
    }
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
        fadePanel.gameObject.SetActive(true);
        fadePanel.Init();
    }

    private void MainSceneSetup()
    {
        fadePanel.StartFadeIn();

        PlayPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    private void PlaySceneSetup()
    {        
        fadePanel.StartFadeIn();

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

    public void FadeOut() => fadePanel.StartFadeOut();
    
    public void MasterVolumeLevel(float sliderVal)
    {
        audioMixer.SetFloat("master", Mathf.Log10(sliderVal) * 20);
    }

    public void BGMVolumeLevel(float sliderVal)
    {
        audioMixer.SetFloat("bgm", Mathf.Log10(sliderVal) * 20);
    }

    public void EffectVolumeLevel(float sliderVal)
    {
        audioMixer.SetFloat("effect", Mathf.Log10(sliderVal) * 20);
    }

}
