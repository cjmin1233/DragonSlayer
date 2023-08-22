using System;
using UnityEngine;
using UnityEngine.UI;
public class StandardSlider : MonoBehaviour, IDefaultUi
{
    [SerializeField] private float smoothTime;
    private float smoothVelocity;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateValue(float curValue, float maxValue)
    {
        if (maxValue <= 0f) return;
        float targetValue = Mathf.Clamp01(curValue / maxValue);
        slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref smoothVelocity, smoothTime);
    }

    public void InitUi()
    {
        slider = GetComponent<Slider>();
    }
}
