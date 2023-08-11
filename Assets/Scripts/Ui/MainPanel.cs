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

    private void Awake()
    {
        startBtn.onClick.AddListener(OnStartBtn);
        recordBtn.onClick.AddListener(OnRecordBtn);
        optionBtn.onClick.AddListener(OnOptionBtn);
        quitBtn.onClick.AddListener(OnQuitBtn);
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
