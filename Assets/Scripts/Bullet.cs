using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    private int _damageAmt;
    
    [SerializeField]
    private Vector2 velocity;
    private const int TilemapLayer = 7;
    private const float lifeSpan = 2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        GameManager.Pause += OnPaused;
        GameManager.Unpause += OnResume;
        StartCoroutine(Lifespan());
    }
    
    private void OnDisable()
    {
        GameManager.Pause -= OnPaused;
        GameManager.Unpause -= OnResume;
    }
    
    private void OnPaused()
    {
        if (_rb is null) return;
        
        _rb.velocity = Vector2.zero;
    }
    
    private IEnumerator ResumeAfterOneFrame()
    {
        yield return null; // Wait for one frame
        if (_rb != null)
        {
            _rb.AddForce(velocity, ForceMode2D.Impulse);
        }
    }

    private void OnResume()
    {
        if (!isActiveAndEnabled) return;
        
        StartCoroutine(ResumeAfterOneFrame());
    }

    public void StartBullet(Vector2 direction, int speed,int damage)
    {
        velocity = direction * speed;
        
        _rb.AddForce(velocity, ForceMode2D.Impulse);
        _damageAmt = damage;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        // if we hit a tile map
        if (collision.gameObject.layer == TilemapLayer)
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
    
    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(lifeSpan);
        gameObject.SetActive(false);
    }
    
}