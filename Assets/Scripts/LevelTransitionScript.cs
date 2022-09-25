using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionScript : MonoBehaviour
{
    private void StartTransition()
    {
    }

    private void OnEnable()
    {
        throw new NotImplementedException();
    }

    /*
    private IEnumerator CoroutineScreenshot()
    {
        
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        
        screenshotTexture.ReadPixels(rect, 0, 0);
        screenshotTexture.Apply();
        
        byte[] byteArray screenshotTexture.EncodeToPNG("Screenshot.png");      
    }
    */
}