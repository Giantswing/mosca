using System;
using System.Collections;
using UnityEngine;

public class WindFxScript : MonoBehaviour
{
    private Action<WindFxScript> _endObj;
    [HideInInspector] public Vector3 moveDir;
    private WaitForSeconds _waitTime;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lifeTime = 1f;

    public float force = 1f;

    private void Start()
    {
        _waitTime = new WaitForSeconds(lifeTime);
    }

    public void Init(Action<WindFxScript> endObj)
    {
        _endObj = endObj;
        force = 1f;
        StartCoroutine(EndObjCoroutine());
    }

    private IEnumerator EndObjCoroutine()
    {
        yield return _waitTime;
        EndObj();
    }

    public void EndObj()
    {
        _endObj(this);
    }

    private void Update()
    {
        transform.position += moveDir * (speed * Time.deltaTime);
        force -= Time.deltaTime / lifeTime;
        transform.Rotate(0, 0, 10f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupArea") || other.CompareTag("Collectable") || other.CompareTag("Fan")) return;

        EndObj();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PickupArea") || other.gameObject.CompareTag("Collectable") ||
            other.gameObject.CompareTag("Fan")) return;

        EndObj();
    }
}