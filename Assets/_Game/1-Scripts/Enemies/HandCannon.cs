using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HandCannon : MonoBehaviour
{
    #region Variables

    #region References

    [SerializeField] [FoldoutGroup("References", false)]
    private Transform firePoint;

    [SerializeField] [FoldoutGroup("References", false)]
    private GameObject bulletPrefab;

    [SerializeField] [FoldoutGroup("References", false)]
    private Transform cannon;

    [SerializeField] [FoldoutGroup("References", false)]
    private Transform cannon3DModel;

    [SerializeField] [FoldoutGroup("References", false)]
    private Transform cannonFuse;

    [SerializeField] [FoldoutGroup("References", false)]
    private ParticleSystem cannonFuseParticles;

    [SerializeField] [FoldoutGroup("References", false)]
    private AudioSource fuseAudioSource;

    [SerializeField] [FoldoutGroup("References", false)]
    private BoxCollider cannonCollider;

    #endregion

    [SerializeField] [HorizontalGroup("Group1", LabelWidth = 60f)]
    private float fireRate = 1f;

    [SerializeField] [HorizontalGroup("Group1", LabelWidth = 95f)]
    private float rotationSpeed = 1f;

    [SerializeField] [HorizontalGroup("Group1", LabelWidth = 95f)]
    private float maxDistance = 10f;

    [SerializeField] [HorizontalGroup("Group2")]
    private float bulletSpeed = 1f;

    [SerializeField] [HorizontalGroup("Group2")]
    private float bulletLifeTime = 1f;

    private bool _canShoot = true;

    private Stack<CannonBullet> _bulletPool = new();
    private float minimumAngleDifference = 4f;
    private Transform target;
    private bool hasTarget = false;

    #endregion

    private void Start()
    {
        InitializeBullets();
        InvokeRepeating(nameof(SelectClosestPlayer), 0.1f, 0.2f);
    }

    private void InitializeBullets()
    {
        for (var i = 0; i < 3; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            CannonBullet cannonBullet = bullet.GetComponent<CannonBullet>();

            bullet.transform.parent = firePoint;
            cannonBullet.cannonParent = this;
            bullet.SetActive(false);

            Physics.IgnoreCollision(cannonCollider, cannonBullet.bulletCollider);

            _bulletPool.Push(cannonBullet);
        }
    }

    private void ResetFuse()
    {
        DOVirtual.DelayedCall(0.4f, () =>
        {
            cannonFuse.localScale = Vector3.one;
            cannonFuseParticles.Play();
            fuseAudioSource.DOFade(.65f, 0.1f);
            fuseAudioSource.Play();


            DOVirtual.DelayedCall(fireRate - 1.4f, () =>
            {
                cannonFuseParticles.Stop();
                fuseAudioSource.DOFade(0, 0.6f);
            });
            cannonFuse.DOScale(Vector3.zero, fireRate);
        });
    }

    private void Update()
    {
        LookAtPlayer();
    }

    private void SelectClosestPlayer()
    {
        hasTarget = true;
        target = TargetGroupControllerSystem.ClosestPlayer(transform);
    }

    private void LookAtPlayer()
    {
        if (!hasTarget) return;
        Vector3 dir = target.position - cannon.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angle = Mathf.LerpAngle(cannon.transform.localRotation.eulerAngles.y, -angle, Time.deltaTime * rotationSpeed);

        cannon.transform.localRotation = Quaternion.Euler(0, angle, 0);

        CheckIfICanShoot(angle);
    }

    private void CheckIfICanShoot(float angle)
    {
        if (!hasTarget) return;
        float angleDiff = Mathf.Abs(cannon.transform.localRotation.eulerAngles.y - angle);
        bool isPlayerInRange = Vector3.Distance(transform.position, target.position) <
                               maxDistance;

        if (angleDiff < minimumAngleDifference && _canShoot && isPlayerInRange)
        {
            Shoot(angle);
            DOVirtual.DelayedCall(fireRate, () => _canShoot = true);
        }
    }

    private void Shoot(float angle)
    {
        _canShoot = false;
        ResetFuse();

        CannonBullet bullet = _bulletPool.Pop();
        bullet.transform.parent = null;
        bullet.gameObject.SetActive(true);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.rb.velocity = firePoint.right * bulletSpeed;
        bullet.Initialize(bulletLifeTime);
        bullet.explosionDirection = new Vector3(0, 0, -angle);

        SoundMaster.PlaySound(firePoint.position, (int)SoundListAuto.CannonShot, true);
        FXMaster.SpawnFX(firePoint.position, (int)FXListAuto.SmokeExplosion, null, new Vector3(0, 0, -angle));
        cannon3DModel.DOPunchPosition(Vector3.left * 0.01f, 0.3f);
        cannon3DModel.DOScale(new Vector3(0.7f, 1f, 1f), 0.3f)
            .OnComplete(() => cannon3DModel.DOScale(new Vector3(1f, 1f, 1f), 0.3f));

        transform.DOShakePosition(0.3f, 0.1f, 10, 90, false, true);
    }

    public void ReturnBulletToStack(CannonBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.parent = firePoint;
        _bulletPool.Push(bullet);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}