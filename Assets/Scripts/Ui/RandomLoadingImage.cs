using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLoadingImage : MonoBehaviour
{
    public Sprite[] sprites;
    public Image image;

    void Start()
    {
        ShowRandomImage();
    }

    private void ShowRandomImage()
    {
        int index = Random.Range(0, sprites.Length);
        Sprite select = sprites[index];
        image.sprite = select;
        Debug.LogFormat("index : {0}, image name : {1}", index, sprites[index].name);
    }
}