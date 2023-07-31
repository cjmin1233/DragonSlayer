using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BtnAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite[] sprites;
    public TextMeshProUGUI textMeshPro;
    

    public void OnPointerDown (PointerEventData downData)
    {
        gameObject.GetComponent<Image>().sprite = sprites[1];
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Bottom;
    }

    public void OnPointerUp (PointerEventData upData)
    {
        gameObject.GetComponent<Image>().sprite = sprites[0];
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
    }
}
