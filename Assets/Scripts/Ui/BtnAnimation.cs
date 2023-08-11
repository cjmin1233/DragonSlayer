using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

public class BtnAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;
    
    private TextMeshProUGUI btnText;
    private Image btnImage;
    private void Awake()
    {
        btnText = GetComponentInChildren<TextMeshProUGUI>();
        btnImage = GetComponent<Image>();
        btnImage.sprite = normalSprite;
    }

    public void OnPointerDown (PointerEventData downData)
    {
        btnImage.sprite = pressedSprite;
        btnText.verticalAlignment = VerticalAlignmentOptions.Bottom;
    }

    public void OnPointerUp (PointerEventData upData)
    {
        btnImage.sprite = normalSprite;
        btnText.verticalAlignment = VerticalAlignmentOptions.Middle;
    }
}
