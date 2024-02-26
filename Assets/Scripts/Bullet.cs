using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;

    private int _damageAmt;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(Decay());
    }

    public void StartBullet(Vector2 direction, int speed,int damage)
    {
        _rb.AddForce(direction * speed, ForceMode2D.Impulse);
        _damageAmt = damage;
    }

    private IEnumerator Decay(float decayRate)
    {
        yield return new WaitForSeconds(decayRate);
        this.gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
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
