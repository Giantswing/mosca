using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MetaScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Material closedMaterial;
    public Material openMaterial;
    private Renderer _myRenderer;
    public bool isOpen = false;
    void Start()
    {
        _myRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenWin()
    {
        isOpen = true;
        _myRenderer.material = openMaterial;
    }
}
