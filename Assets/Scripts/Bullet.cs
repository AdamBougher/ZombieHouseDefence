using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    private int _damageAmt;
    
    private Vector2 _pausedVelocity;

    private void OnEnable() {
        _rb = GetComponent<Rigidbody2D>();
        GameManager.Pause += OnPaused;
        GameManager.Unpause += OnResume;
    }
    
    private void OnDestroy()
    {
        GameManager.Pause -= OnPaused;
        GameManager.Unpause -= OnResume;
    }
    
    private void OnPaused()
    {
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
        gameObject.SetActive(false);
    }
}
