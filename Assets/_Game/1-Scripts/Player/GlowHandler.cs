using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GlowHandler : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer playerMesh;
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowState = Shader.PropertyToID("_GlowState");
    public static GlowHandler instance;

    private void Awake()
    {
        instance = this;
        playerMesh = GetComponent<STATS>().myRenderer.GetComponent<SkinnedMeshRenderer>();
    }

    public void Glow(Color color)
    {
        playerMesh.material.SetFloat(GlowState, 1f);
        playerMesh.material.SetColor(GlowColor, color);
        playerMesh.material.DOFloat(0, GlowState, 0.2f).SetDelay(0.1f);
    }

    public void Glow(Color color, float duration)
    {
        playerMesh.material.SetFloat(GlowState, 1f);
        playerMesh.material.SetColor(GlowColor, color);
        playerMesh.material.DOFloat(0, GlowState, 0.2f).SetDelay(duration);
    }

    public void Glow()
    {
        Glow(Color.green);
    }

    public static void GlowStatic(Color color)
    {
        instance.Glow(color);
    }
}