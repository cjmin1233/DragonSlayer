using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image progressBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }
    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f)
            {
                progressBar.rectTransform.localScale = new Vector3 (op.progress, 1, 1);
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                float duration = 5.0f; // fake로딩 지속시간
                float t = Mathf.Clamp01(timer / duration);
                
                progressBar.rectTransform.localScale = new Vector3 (Mathf.Lerp(0.9f, 1f, t), 1, 1);
                if (progressBar.rectTransform.localScale.x >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
