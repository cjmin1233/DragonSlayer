using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ClearTimeManager;

[System.Serializable]
public class ClearTimeDataList
{
    public List<ClearTimeData> clearTimes;
}

public class ClearTimeManager : MonoBehaviour
{
    public List<ClearTimeData> clearTimeList;
    public int maxEntries = 5;

    void Awake()
    {
        LoadClearTimesFromPlayerPrefs();
    }

    public void AddClearTime(float clearTime)
    {
        ClearTimeData newEntry = new ClearTimeData
        {
            clearTime = clearTime
        };

        clearTimeList.Add(newEntry);
        clearTimeList.Sort((a, b) => a.clearTime.CompareTo(b.clearTime));

        if (clearTimeList.Count > maxEntries)
        {
            clearTimeList.RemoveAt(clearTimeList.Count - 1);
        }

        SaveClearTimeToPlayerPrefs();
    }

    public void SaveClearTimeToPlayerPrefs()
    {
        string jsonData = JsonUtility.ToJson(new ClearTimeDataList { clearTimes = clearTimeList });
        PlayerPrefs.SetString("ClearTimes", jsonData);
    }

    public void LoadClearTimesFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("ClearTimes"))
        {
            string jsonData = PlayerPrefs.GetString("ClearTimes");
            ClearTimeDataList data = JsonUtility.FromJson<ClearTimeDataList>(jsonData);
            clearTimeList = data.clearTimes;
        }
        else
        {
            clearTimeList = new List<ClearTimeData>();
        }
    }
}


