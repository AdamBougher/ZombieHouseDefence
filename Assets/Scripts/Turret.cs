using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    [Header("Optional Visual Overrides")]
    [Tooltip("Sprite for the weapon arm (optional)")]
    [SerializeField] private Sprite weaponSprite;

    // you can leave these null in the Inspector to auto‐find:
    [SerializeField] private SpriteRenderer weaponRenderer;

    [Header("Turret Settings")]
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float fireRate   = 0.75f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private int   maxAngle   = 45;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AudioClip fireSound;

    private enum State { Searching, Targeting }

    private State state;
    private Enemy currentTarget;
    private Coroutine fireRoutine;

    private Transform weaponTf;
    private AudioSource audioSrc;

    private void Awake()
    {
        // 1) cache AudioSource
        audioSrc = GetComponent<AudioSource>();

        // 2) find your weapon child by name
        var w = transform.Find("weapon");
        weaponTf = w != null ? w : transform;

        // 4) weapon sprite swap
        if (weaponRenderer == null)
            weaponRenderer = weaponTf.GetComponent<SpriteRenderer>();
        if (weaponRenderer != null && weaponSprite != null)
            weaponRenderer.sprite = weaponSprite;

        // 5) initial state
        state = State.Searching;
    }

    private void Start()
    {
        StartCoroutine(RotateLoop());
    }

    private void Update()
    {
        var hitEnemy = TryScanForEnemy();
        if (hitEnemy != null && currentTarget == null)
            SetTarget(hitEnemy);

        if (!IsTargetValid())
            ClearTarget();
    }

    private Enemy TryScanForEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            weaponTf.position,
            weaponTf.right,
            rayDistance,
            enemyMask
        );
        return hit.collider?.GetComponent<Enemy>();
    }

    private void SetTarget(Enemy e)
    {
        currentTarget = e;
        state = State.Targeting;
        fireRoutine = StartCoroutine(FireLoop());
    }

    private void ClearTarget()
    {
        if (currentTarget == null) return;
        currentTarget = null;
        state = State.Searching;
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    private bool IsTargetValid()
    {
        if (currentTarget == null) return false;
        if (!currentTarget.gameObject.activeInHierarchy) return false;

        // Check if still within firing arc
        Vector3 dir = (currentTarget.transform.position - weaponTf.position).normalized;
        float angle = Vector3.Angle(weaponTf.right, dir);
        return angle <= maxAngle;
    }

    private IEnumerator RotateLoop()
    {
        float sweepTime = 0f;
        float twoAngle  = maxAngle * 2f;

        while (true)
        {
            if (state == State.Searching)
            {
                sweepTime += Time.deltaTime * rotationSpeed;
                float z = Mathf.PingPong(sweepTime, twoAngle) - maxAngle;
                weaponTf.rotation = Quaternion.Euler(0, 0, z);
            }
            else // targeting
            {
                Vector3 dir = (currentTarget.transform.position - weaponTf.position).normalized;
                float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                z = Mathf.Clamp(z, -maxAngle, +maxAngle);
                weaponTf.rotation = Quaternion.Lerp(
                    weaponTf.rotation,
                    Quaternion.Euler(0, 0, z),
                    Time.deltaTime * rotationSpeed
                );
            }
            yield return null;
        }
    }

    private IEnumerator FireLoop()
    {
        while (currentTarget != null)
        {
            yield return new WaitWhile(() => GameManager.GamePaused);
            audioSrc.PlayOneShot(fireSound);

            if (BulletPool.SharedInstance.GetPooledObject()
                .TryGetComponent(out Bullet b))
            {
                b.transform.SetPositionAndRotation(
                    weaponTf.position, weaponTf.rotation
                );
                b.gameObject.SetActive(true);
                b.StartBullet(weaponTf.right, 50, 1);
            }

            yield return new WaitWhile(() => GameManager.GamePaused);
            yield return new WaitForSeconds(fireRate);
        }
    }
}
