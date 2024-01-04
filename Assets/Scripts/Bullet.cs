using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public double DecayRate;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(Decay());
    }

    public void SetVelocity(Vector2 direction, int speed)
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds((float)DecayRate);
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.otherCollider.name);
    }
}
