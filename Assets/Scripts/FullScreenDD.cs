using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenDD : MonoBehaviour
{
    public void FullScreenChange(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreen = true; break;

            case 1:
                Screen.fullScreen = false; break;
        }
    }
}
