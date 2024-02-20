using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(collision.TryGetComponent(out Enemy e))
        {
            Damage(e);
        }
        
    }

    private void Damage(Enemy target)
    {
        target.Damage(_damageAmt);
        this.gameObject.SetActive(false);
    }
}
