using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        gameObject.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTooltip(Vector3 pos, string text)
    {
        gameObject.SetActive(true);
        rectTransform.position = pos;
        textMeshProUGUI.text = text;
    }

    public void Disable() => gameObject.SetActive(false);
}
