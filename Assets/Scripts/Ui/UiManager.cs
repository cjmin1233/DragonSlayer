using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    private MyDefaultInputAction myInputAction;
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject eventSystem;

    [SerializeField] private MainPanel mainPanel;
    [SerializeField] private PlayPanel playPanel;
    
    public GameObject GameOverPanel;
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

    private DefaultInputActions inputAction;
    private int popUpCounter = 0;

    private void Start()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        eventSystem.SetActive(true);
        DontDestroyOnLoad(gameObject);
        
        // add listener
        GameManager.Instance.onMainSceneLoaded.AddListener(MainSceneSetup);
        GameManager.Instance.onPlaySceneLoaded.AddListener(PlaySceneSetup);
        //
        
        fadePanel.gameObject.SetActive(true);
        fadePanel.Init();

        InitInputAction();
        playPanel.InitPanel();
        
        MainSceneSetup();
    }

    private void InitInputAction()
    {
        inputAction = new DefaultInputActions();
        inputAction.UI.Enable();

        inputAction.UI.Cancel.started += OnEscapeTrigger;

        myInputAction = new MyDefaultInputAction();
        myInputAction.UiInput.Enable();

        myInputAction.UiInput.Inventory.started += OnInventoryTrigger;
    }
    
    private void Update()
    {
        if (playPanel.gameObject.activeSelf)
        {
            playPanel.UpdateUi();
        }
    }

    private void OnEscapeTrigger(InputAction.CallbackContext context)
    {
        if (popUpCounter <= 0 && playPanel.gameObject.activeSelf
                              && GameManager.Instance.gameState.Equals(GameState.Running))
        {
            popUpCounter = 1;
            PauseMenu.SetActive(true);
            GameManager.Instance.PauseGame(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void OnInventoryTrigger(InputAction.CallbackContext context)
    {
        if (playPanel.gameObject.activeSelf) playPanel.ToggleInventory();
    }

    public void OnPopupUiDisable()
    {
        popUpCounter--;
        if (popUpCounter <= 0 && playPanel.gameObject.activeSelf)
        {
            popUpCounter = 0;
            GameManager.Instance.PauseGame(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void MainSceneSetup()
    {
        fadePanel.StartFadeIn();

        playPanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(false);
        mainPanel.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
    }
    private void PlaySceneSetup()
    {        
        fadePanel.StartFadeIn();

        mainPanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(false);
        playPanel.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GameOverSetup()
    {
        mainPanel.gameObject.SetActive(false);
        playPanel.gameObject.SetActive(false);
        GameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
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
    public void FadeIn() => fadePanel.StartFadeIn();

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BgmVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }

    public void GetItem2Inventory(ItemScriptableObject itemData) => playPanel.GetItem2Inventory(itemData);
    public void ShowInteractInfo(string description) => playPanel.ShowInteractInfo(description);
    public void HideInteractInfo() => playPanel.HideInteractInfo();
}
