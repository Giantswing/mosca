using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BombScript : CollectableBehaviour
{
    private Vector3 _startPosition;
    private Transform _parentTransform;
    private bool _hasParent = false;
    private bool _isFlashing = false;

    [SerializeField] private Rigidbody myRigidbody;
    [SerializeField] private SmartData.SmartVector3.Vector3Reader playerDir;
    [SerializeField] private Collider collisionCollider;
    private WaitForSeconds _enableCollisionWait = new(0.03f);
    [SerializeField] private PlayerReferenceSO playerReference;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float thresholdForce = 4f;
    private float _currentLifeTime = 0f;
    private bool _canExplode = false;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private Material flashingMaterial;
    [SerializeField] private Material defaultMaterial;

    private WaitForSeconds _slowFlash = new(0.5f);
    private WaitForSeconds _midFlash = new(0.25f);
    private WaitForSeconds _fastFlash = new(0.1f);
    private WaitForSeconds _flashDuration = new(0.15f);

    private MeshRenderer[] _meshRenderers;

    [SerializeField] private SimpleAudioEvent beepSound;


    private new void Start()
    {
        base.Start();
        _startPosition = transform.position;

        _parentTransform = transform.parent;

        if (_parentTransform != null) _hasParent = true;

        collisionCollider.enabled = false;
        _currentLifeTime = lifeTime;

        _meshRenderers = displayObject.GetComponentsInChildren<MeshRenderer>();
    }

    private IEnumerator FlashMaterial()
    {
        foreach (var renderer in _meshRenderers)
            renderer.material = flashingMaterial;

        //map the pitch to the life time
        beepSound.pitch.minValue = Mathf.Lerp(1.2f, .8f, _currentLifeTime / lifeTime);
        beepSound.pitch.maxValue = beepSound.pitch.minValue;
        GlobalAudioManager.PlaySound(beepSound, transform.position);

        yield return _flashDuration;

        foreach (var renderer in _meshRenderers)
            renderer.material = defaultMaterial;


        if (_currentLifeTime > lifeTime / 2f)
            yield return _slowFlash;
        else if (_currentLifeTime > lifeTime / 3.5f)
            yield return _midFlash;
        else
            yield return _fastFlash;


        if (_isFlashing)
            StartCoroutine(FlashMaterial());
    }

    private new void Update()
    {
        base.Update();

        if (isFollowing != 0)
        {
            _currentLifeTime -= Time.deltaTime;

            if (_isFlashing == false)
            {
                _isFlashing = true;
                StartCoroutine(FlashMaterial());
            }
        }

        if (_currentLifeTime <= 0f)
            Explode();
    }


    public void Throw()
    {
        if (isFollowing != 3) return;

        isFollowing = -1;
        _tweener.Kill();
        transform.DOKill();

        RemoveHolder();

        myRigidbody.useGravity = true;
        myRigidbody.velocity = playerReference.playerRigidbody.velocity * 3f;
        displayObject.DOKill();

        StartCoroutine(BombCoroutine());
    }

    private IEnumerator BombCoroutine()
    {
        yield return _enableCollisionWait;
        collisionCollider.enabled = true;
        _canExplode = true;
    }

    public void Reset()
    {
        if (isFollowing == 0) return;

        RemoveHolder();

        if (!_hasParent)
        {
            transform.position = _startPosition;
        }
        else
        {
            transform.SetParent(_parentTransform);
            transform.position = _parentTransform.position;
        }

        isFollowing = 0;
        _whoToFollow = null;
        collisionCollider.enabled = false;
        myRigidbody.useGravity = false;
        transform.rotation = Quaternion.identity;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        _canExplode = false;
        Initialize();
    }

    public void Explode()
    {
        _currentLifeTime = lifeTime;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        _isFlashing = false;
        FreezeFrameScript.ShakeCamera(.7f, 4f);
        Reset();

        //myParticles.Emit(40);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(other.collider, collisionCollider, true);
        }
        else
        {
            if (_canExplode)
                if (Mathf.Abs(myRigidbody.velocity.magnitude) > thresholdForce)
                    Explode();
        }
    }
}