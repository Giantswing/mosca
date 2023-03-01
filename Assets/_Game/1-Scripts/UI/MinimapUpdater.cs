using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapUpdater : MonoBehaviour
{
    public int fps = 2;
    [SerializeField] private RenderTexture rt;
    [SerializeField] private Transform player;
    private Camera cam;
    private float elapsed;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    private void Update()
    {
        //transform.position = new Vector3(player.position.x, transform.position.y, player.position.z - 30f);

        cam.targetTexture = null;
        elapsed += Time.deltaTime;
        if (elapsed > .2f)
        {
            elapsed = 0;
            cam.targetTexture = rt;
            cam.Render();
        }
    }
}