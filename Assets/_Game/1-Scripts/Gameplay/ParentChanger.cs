using UnityEngine;

public class ParentChanger : MonoBehaviour
{
    [SerializeField] private float timeInMax = 0.2f;
    [SerializeField] private bool isPlayerIn;
    [SerializeField] private bool changedParent;
    [SerializeField] private Transform otherTransform;
    private EventCaller _eventCaller;

    private float _timeIn;

    // Start is called before the first frame update
    private void Start()
    {
        _eventCaller = GetComponentInParent<EventCaller>();
        timeInMax = 0.2f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlayerIn && !changedParent)
            _timeIn += Time.deltaTime;
        if (_timeIn > timeInMax)
        {
            otherTransform.parent = transform;
            changedParent = true;
            _timeIn = 0;
            _eventCaller.OnStartEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherTransform = other.transform;
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
            isPlayerIn = false;
            changedParent = false;
            _timeIn = 0;
        }
    }

    public void SetTimeMax(float time)
    {
        timeInMax = time;
    }
}