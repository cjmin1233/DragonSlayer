using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임매니저 스크립트에서 public FadeUI fadeUI를 이용해서 불러오고 효과를 줄 곳에서
//fadeUI.gameObject.SetActive(true) 후에 fadeUI.StartFadeIn/Out을 실행
//코루틴 안에 있는 SetActive(false)/WaitForSeconds(5)는 test용 이므로 없어두 됨
public class FadeUI : MonoBehaviour
{
    public Image fadeUI;

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeAlpha = 0;
        while (fadeAlpha < 1.0f)
        {
            fadeAlpha += 0.01f;
            yield return new WaitForSeconds(0.01f);
            fadeUI.color = new Color(0, 0, 0, fadeAlpha);
        }
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }

    IEnumerator FadeInCoroutine()
    {
        float fadeAlpha = 1;
        while (fadeAlpha > 0f)
        {
            fadeAlpha -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            fadeUI.color = new Color(0, 0, 0, fadeAlpha);
        }
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }
}
