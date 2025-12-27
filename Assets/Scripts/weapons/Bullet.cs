using UnityEngine;

namespace ZombieHouseDefense.Weapon
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 50f;
        [SerializeField] private float _lifetime = 1f;
        [SerializeField] private float _offScreenGrace = 0.5f;
        [SerializeField] private float _screenMargin = 0f;
        private float _lifeTimer;
        private float _offScreenTimer;

        private void OnEnable()
        {
            _lifeTimer = _lifetime;
            _offScreenTimer = 0f;
        }

        private void Update()
        {
            transform.position += _speed * Time.deltaTime * transform.right;

            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }

            var cam = Camera.main;
            if (cam != null)
            {
                var vp = cam.WorldToViewportPoint(transform.position);
                bool onScreen = vp.z > 0f &&
                                vp.x >= 0f - _screenMargin && vp.x <= 1f + _screenMargin &&
                                vp.y >= 0f - _screenMargin && vp.y <= 1f + _screenMargin;

                if (!onScreen)
                {
                    _offScreenTimer += Time.deltaTime;
                    if (_offScreenTimer >= _offScreenGrace)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    _offScreenTimer = 0f;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleHit(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleHit(other);
        }

        private void HandleHit(Component other)
        {
            var go = other.gameObject;

            if(go.CompareTag("Player"))
            {
                return;
            }

            Debug.Log("Bullet collided with " + go.name);

                if (go.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(1);
            }

            gameObject.SetActive(false);
        }
    }
}
