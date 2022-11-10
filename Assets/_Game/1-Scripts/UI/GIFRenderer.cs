using System;
using UnityEngine;
using UnityEngine.UI;

public class GIFRenderer : MonoBehaviour
{
    //gif renderer
    public Texture2D[] frames;
    public int framesPerSecond = 10;
    public bool loop = true;
    public bool playOnAwake = true;

    private int index = 0;
    private float time = 0;
    private bool isPlaying = false;

    private RawImage renderer;


    private void Start()
    {
        if (playOnAwake) Play();

        renderer = GetComponent<RawImage>();
    }

    private void Play()
    {
        //gif renderer
        isPlaying = true;
        index = 0;
        time = 0;
    }

    private void Update()
    {
        if (isPlaying)
        {
            time += Time.unscaledDeltaTime;
            if (time >= 1f / framesPerSecond)
            {
                time = 0;
                index++;
                if (index >= frames.Length)
                {
                    if (loop)
                        index = 0;
                    else
                        isPlaying = false;
                }
            }
        }

        if (frames.Length > 0) renderer.texture = frames[index];
    }
}