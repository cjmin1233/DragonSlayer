using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public FadeUI fadeUI;

    public GameObject recordPanel;
    public GameObject optionPanel;
    public GameObject quitPanel;

    public AudioMixer mixer;

    public void OnStartBtn()
    {
        Debug.Log("버튼클릭");
        fadeUI.gameObject.SetActive(true);
        fadeUI.StartFadeOut();
        //SceneManager.LoadScene("Main");
    }

    public void OnRecordBtn() 
    {
        recordPanel.transform.localPosition += new Vector3(0, 1000, 0);
    }

    public void OnRecordQuit()
    {
        recordPanel.transform.localPosition += new Vector3(0, -1000, 0);
    }

    public void OnOptionBtn()
    {
        optionPanel.transform.localPosition += new Vector3(-700, 1000, 0);
    }
    public void MasterVolumeLevel(float sliderVal)
    {
        mixer.SetFloat("master", Mathf.Log10(sliderVal) * 20);
    }

    public void BGMVolumeLevel(float sliderVal)
    {
        mixer.SetFloat("bgm", Mathf.Log10(sliderVal) * 20);
    }

    public void EffectVolumeLevel(float sliderVal)
    {
        mixer.SetFloat("effect", Mathf.Log10(sliderVal) * 20);
    }

    public void OnOptionQuit()
    {
        optionPanel.transform.localPosition += new Vector3(700, -1000, 0);
    }

    public void OnQuitBtn()
    {
        quitPanel.transform.localPosition += new Vector3(-1400, 1000, 0);
        //Application.Quit();
    }

    public void OnGameQuit()
    {
        Application.Quit();
    }

    public void OnQuitQuit()
    {
        quitPanel.transform.localPosition += new Vector3(1400, -1000, 0);
    }
}
