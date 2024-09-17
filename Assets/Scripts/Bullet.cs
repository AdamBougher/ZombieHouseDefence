using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;
    private int _damageAmt;
    private float bulletLifespan = 3;
    
    private Vector2 _pausedVelocity;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnBecameInvisible() {
        if (gameObject.activeSelf) {
            StartCoroutine(Timer());
        }
    }

    private void OnEnable() {
        GameManager.Pause += OnPaused;
        GameManager.Unpause += OnResume;
    }
    
    private void OnPaused() {
        _rb ??= GetComponent<Rigidbody2D>();
        
        _pausedVelocity = _rb.velocity;
        _rb.velocity = Vector2.zero;

    }
    
    private void OnResume()
    {
        _rb.velocity = _pausedVelocity;
    }

    public void StartBullet(Vector2 direction, int speed,int damage)
    {
        _rb.AddForce(direction * speed, ForceMode2D.Impulse);
        _damageAmt = damage;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        
        // if we hit a tile map
        if (collision.gameObject.layer == 7)
        {
            gameObject.SetActive(false);
        }

        if (!collision.TryGetComponent(out IHittable hit)) return;
        
        Damage(hit);
    }

    private void Damage(IHittable target)
    {
        target.Damage(_damageAmt);
        //if Timer is running stop it
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
