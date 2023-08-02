using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BtnAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite[] sprites; //[일반버튼스프라이트,눌린버튼스프라이트]
    public TextMeshProUGUI textMeshPro; //각 버튼의 텍스트메시프로
    

    public void OnPointerDown (PointerEventData downData) //마우스클릭을 시작할때 스프라이트 변경하고 글자도 아래로 내려가게
    {
        gameObject.GetComponent<Image>().sprite = sprites[1]; 
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Bottom;
    }

    public void OnPointerUp (PointerEventData upData) //마우스클릭을 뗐을때 스프라이트 다시 변경하고 글자 원위치
    {
        gameObject.GetComponent<Image>().sprite = sprites[0];
        textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
    }
}
