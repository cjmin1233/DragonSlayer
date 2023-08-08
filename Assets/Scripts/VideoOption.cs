using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{

    FullScreenMode screenMode;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenBtn;
    List<Resolution> resolutions = new List<Resolution>();
    int resolutionNum;

    static int GCD(int num1, int num2)
    {
        int remainder;
        while (num2 != 0)
        {
            remainder = num1 % num2;
            num1 = num2;
            num2 = remainder;
        }
        return num1;
    }

    void Start()
    {
        InitUI();
    }

    private void InitUI()
    {
        resolutions.Clear(); // 기존의 resolutions 리스트를 초기화

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            int MAX = GCD(Screen.resolutions[i].width, Screen.resolutions[i].height);
            if ((Screen.resolutions[i].width / MAX == 16) && (Screen.resolutions[i].height / MAX == 9))
                resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            screenMode);
    }
}
