using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private turretWeapon _turretWeapon;
    private GameObject WeaponObject => transform.childCount > 0 ? transform.GetChild(0).gameObject : null;
    
    [ShowInInspector]
    private Enemy _target;
    
    private Coroutine _fireAtTargetCoroutine;

    private bool _isFiring;

    private Enemy Target {
        get => _target;
        set {
            _target = value;
            if (_target is null)
            {
                state = State.Searching;
                _isFiring = false;
            }else{
                Debug.Log("Firing at " + value.name);
                state = State.Targeting;
                _isFiring = true;
                _fireAtTargetCoroutine ??= StartCoroutine(FireAtTarget());
            }
        }
    }
    
    private LineRenderer _lineRenderer;
    
    private AudioSource AudioSource => GetComponent<AudioSource>();
    
    private static ObjectPool<Bullet> BulletPool => global::BulletPool.SharedInstance;
    
    public AudioClip fire;
   
    [SerializeField]
    private State state;
    
    public int rotationDegree;
    
    private void Start() {
        SetWeapon();
        
        StartCoroutine(HandleRotation());
        state=State.Searching;
    }

    private void SetWeapon(turretWeapon turretWeapon = null) {
        if (turretWeapon is null) {
            _turretWeapon = new(
                10,
                10, 
                0.75f,            
                Resources.Load<Sprite>("icons/ak47"),
                fire
            );
        }
        else
        {
            _turretWeapon = turretWeapon;
        }
        
        WeaponObject.GetComponent<SpriteRenderer>().sprite = _turretWeapon.WeaponSprite;
        
    }

    private void Update() {
        HandleHitResult(PerformRaycast());
        
        if (!IsTargetActive()){
            Target = null;
        }
        
    }

    private RaycastHit2D PerformRaycast() {
        return Physics2D.Raycast(
            WeaponObject.transform.position,
            WeaponObject.transform.right,
            _turretWeapon.RayDistance
        );
    }

    private void HandleHitResult(RaycastHit2D hit) {
        var hitCollider = hit.collider;

        if (hitCollider is null ||
            _target is not null ||
            !hitCollider.TryGetComponent<Enemy>(out var enemy)
            ) 
            return;
            
        Target = enemy;
    }

    private void Projectile(Transform spawn) {
        AudioSource.PlaySound(_turretWeapon.WeaponSound);

        if (BulletPool.GetPooledObject().TryGetComponent<Bullet>(out var bullet))
        {
            bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            bullet.gameObject.SetActive(true);
        }

        bullet.StartBullet(spawn.right, 50, 1);
    }
    
    private IEnumerator HandleRotation() {
        float time = 0;
        float doubleRotationDegree = rotationDegree * 2;

        while (true)
        {
            switch (state)
            {
                case State.Searching:
                    HandleSearchingRotation(ref time, doubleRotationDegree);
                    break;

                case State.Targeting:
                    HandleTargetingRotation();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return null;
        }
    }

    private void HandleSearchingRotation(ref float time, float doubleRotationDegree) {
        time += Time.deltaTime * _turretWeapon.RotationSpeed;
        float targetRotation = Mathf.PingPong(time, doubleRotationDegree) - rotationDegree;
        targetRotation = Mathf.Clamp(targetRotation, -rotationDegree, rotationDegree);
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, targetRotation);

        WeaponObject.transform.rotation = Quaternion.Lerp(
            WeaponObject.transform.rotation,
            targetQuaternion,
            Time.deltaTime * _turretWeapon.RotationSpeed
        );
    }

    private void HandleTargetingRotation() {
        Vector3 targetPosition = Target!.transform.position;
        Vector3 direction = targetPosition - WeaponObject.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Clamp the angle within the range of -rotationDegree and rotationDegree
        angle = Mathf.Clamp(angle, -rotationDegree, rotationDegree);

        // If the target is outside the range, set _target to null
        if (Mathf.Abs(angle) > rotationDegree) {
            Target = null;
        }else{
            Quaternion targetQuaternion = Quaternion.Euler(0, 0, angle);
            WeaponObject.transform.rotation = Quaternion.Lerp(
                WeaponObject.transform.rotation,
                targetQuaternion,
                Time.deltaTime * _turretWeapon.RotationSpeed
            );
        }
    }
    
   private IEnumerator FireAtTarget() {
        while (true)
        {
            if (_isFiring && Target is not null)
            {
                yield return new WaitWhile(() => GameManager.GamePaused);

                Projectile(WeaponObject.transform);
                
                yield return new WaitWhile(() => GameManager.GamePaused);
                yield return new WaitForSeconds(_turretWeapon.FireRate);
            }else{
                yield return null;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private bool IsTargetActive() {
        return Target is not null && Target.gameObject.activeInHierarchy;
    }
    
    private enum State {
        Searching,
        Targeting
    }
}
