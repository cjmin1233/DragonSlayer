using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private RectTransform rect;
    private TextMeshProUGUI toolTipText;

    private void Start()
    {       
        gameObject.SetActive(false);
        rect = GetComponent<RectTransform>();
        toolTipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTooltip(Vector3 pos, string text)
    {
        gameObject.SetActive(true);
        rect.position = pos;
        toolTipText.text = text;
    }

    public void Disable() => gameObject.SetActive(false);
}
