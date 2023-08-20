using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class RenderTextureShutter : MonoBehaviour
{
    public bool trigger;
    public RenderTexture DrawTexture;
    public string relativePath = "/Resources/Texture";

    [SerializeField] private GameObject[] targets;

    private void Start()
    {
        StartCoroutine(AutoCapture());
    }

    void RenderTextureSave(GameObject target)
    {
        RenderTexture.active = DrawTexture;
        var texture2D = new Texture2D(DrawTexture.width, DrawTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, DrawTexture.width, DrawTexture.height), 0, 0);
        texture2D.Apply();
        var data = texture2D.EncodeToPNG();
        // File.WriteAllBytes("C:/Example/Image.png", data);
        File.WriteAllBytes(Application.dataPath+relativePath+"/"+$"{target.name}Image.png", data);
    }

    IEnumerator AutoCapture()
    {
        foreach (var target in targets)
        {
            target.SetActive(true);
            yield return new WaitForSeconds(2.75f);
            RenderTextureSave(target);
            yield return new WaitForSeconds(2f);
            target.SetActive(false);
        }
    }
}
