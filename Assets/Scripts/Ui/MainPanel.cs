using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject recordMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject quitMenu;

    [SerializeField] private Button startBtn;
    [SerializeField] private Button recordBtn;
    [SerializeField] private Button optionBtn;
    [SerializeField] private Button quitBtn;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;

    private void Start()
    {
        startBtn.onClick.AddListener(OnStartBtn);
        recordBtn.onClick.AddListener(OnRecordBtn);
        optionBtn.onClick.AddListener(OnOptionBtn);
        quitBtn.onClick.AddListener(OnQuitBtn);

        if (!PlayerPrefs.HasKey("MasterVolume")) UiManager.Instance.SetMasterVolume(1f);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (!PlayerPrefs.HasKey("BgmVolume")) UiManager.Instance.SetBgmVolume(.75f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        if (!PlayerPrefs.HasKey("EffectVolume")) UiManager.Instance.SetEffectVolume(.75f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume");
    }

    private void OnStartBtn()
    {
        UiManager.Instance.Attempt2LoadNextScene();
    }

    private void OnRecordBtn()
    {
        mainMenu.SetActive(false);
        recordMenu.SetActive(true);
    }

    private void OnOptionBtn()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    private void OnQuitBtn()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(true);
    }
}
