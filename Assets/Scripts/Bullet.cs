using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    private int DamageAmt;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(Decay());
    }

    public void StartBullet(Vector2 direction, int speed,int damage)
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        DamageAmt = damage;
    }

    private IEnumerator Decay(float DecayRate)
    {
        yield return new WaitForSeconds(DecayRate);
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
        target.Damage(DamageAmt);
        this.gameObject.SetActive(false);
    }
}
