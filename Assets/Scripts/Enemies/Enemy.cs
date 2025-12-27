using Sirenix.OdinInspector;
using UnityEngine;
using ZombieHouseDefense.Core;

namespace ZombieHouseDefense.Enemy
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : Character, ZombieHouseDefense.Interfaces.IDamageable
    {
        [SerializeField, BoxGroup("Character")] private string enemyType;
        [SerializeField] private int damageAmount = 1;
        [SerializeField] private Vector2 cryIntervalRange = new(2f, 15f);
        [SerializeField] private bool playCryOnEnable = true;

        [SerializeField] private GameObject target;

        [SerializeField, BoxGroup("components")] private SpriteRenderer sprite;
        [SerializeField, BoxGroup("components")] private Collider2D Hitbox, TriggerBox;

        private UnityEngine.AI.NavMeshAgent agent;
        private Rigidbody2D _rb;

        private Vector2 _lastLookDir = Vector2.right;
        private Vector2 _desiredVelocity;

        private Coroutine _cryRoutine;

        public AudioClip cry, deathSound;

        void Start()
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            // IMPORTANT: Agent is pathfinding only; Rigidbody2D does the actual movement/collisions.
            agent.updatePosition = false;

            agent.speed = Speed;

            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        void Update()
        {
            if (target == null)
                return;

            Vector3 currentPos = transform.position;
            Vector3 targetPos = target.transform.position;

            agent.SetDestination(targetPos);

            // Cache desired velocity for physics step.
            _desiredVelocity = Vector2.ClampMagnitude((Vector2)agent.desiredVelocity, agent.speed);

            // Face target
            Vector2 dir = (Vector2)(targetPos - currentPos);
            float dirSqrMag = dir.sqrMagnitude;

            if (dirSqrMag > 0.0001f)
                _lastLookDir = dir.normalized;

            if (_lastLookDir.sqrMagnitude < 0.0001f)
                return;

            float angle = Mathf.Atan2(_lastLookDir.y, _lastLookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void FixedUpdate()
        {
            // Move using physics so Door BoxCollider2D can block the enemy.
            _rb.linearVelocity = _desiredVelocity;

            // Keep the agent synced to the physics position.
            agent.nextPosition = _rb.position;
        }


        private void OnEnable()
        {
            if (playCryOnEnable)
            {
                StartCryLoop();
            }
        }

        private void OnDisable()
        {
            if (_cryRoutine != null)
            {
                StopCoroutine(_cryRoutine);
                _cryRoutine = null;
            }
        }

        protected override void Die()
        {
            // Enemy-specific die behavior
            Debug.Log($"{gameObject.name} (Enemy) has died.");
            StartCoroutine(Death());
        }

        public override void Damage(int amt)
        {
            base.Damage(amt);
            if (deathSound != null && AudioSource != null)
            {
                AudioSource.PlayOneShot(deathSound);
            }
            
        }

        private void StartCryLoop()
        {
            if (_cryRoutine != null)
            {
                StopCoroutine(_cryRoutine);
            }
            _cryRoutine = StartCoroutine(CryLoop());
        }

        private System.Collections.IEnumerator CryLoop()
        {
            while (enabled && gameObject.activeInHierarchy)
            {
                if (cry != null && AudioSource != null)
                {
                    AudioSource.PlayOneShot(cry);
                }

                float min = Mathf.Max(0.01f, Mathf.Min(cryIntervalRange.x, cryIntervalRange.y));
                float max = Mathf.Max(min, Mathf.Max(cryIntervalRange.x, cryIntervalRange.y));
                float wait = UnityEngine.Random.Range(min, max);
                yield return new WaitForSeconds(wait);
            }

            _cryRoutine = null;
        }

        private System.Collections.IEnumerator Death()
        {
            sprite.enabled = false;
            Hitbox.enabled = false;
            TriggerBox.enabled = false;

            AudioSource.Stop();
            AudioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(deathSound.length);
            this.gameObject.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D other)
        {   
            Debug.Log("Enemy collided with " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                ZombieHouseDefense.Interfaces.IDamageable player = other.gameObject.GetComponent<ZombieHouseDefense.Interfaces.IDamageable>();
                player?.Damage(damageAmount);
            }
        }

        
    }
}
