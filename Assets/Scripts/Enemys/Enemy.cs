using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using ZombieHouseDefense.Core;
using ZombieHouseDefense.Interfaces;
namespace ZombieHouseDefense.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Character,  IDamageable
    {   
        [SerializeField, BoxGroup("Character")] private string enemyType;
        [SerializeField] private int damageAmount = 1;
        [SerializeField] private Vector2 cryIntervalRange = new(2f, 15f);
        [SerializeField] private bool playCryOnEnable = true;

        [SerializeField] private GameObject target;
        
        [SerializeField, BoxGroup("components")] private SpriteRenderer sprite;
        [SerializeField, BoxGroup("components")] private Collider2D Hitbox, TriggerBox;

        private NavMeshAgent agent;
        private Vector2 _lastLookDir = Vector2.right;

        private Coroutine _cryRoutine;

        public AudioClip cry, deathSound;

	    void Start()	{
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = Speed;
        }

        void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector3 currentPos = transform.position;
            Vector3 targetPos = target.transform.position;

            // Keep the agent moving toward the target.
            agent.SetDestination(targetPos);

            // Face the target using a simple world-space direction to avoid camera lookups.
            Vector2 dir = (Vector2)(targetPos - currentPos);
            float dirSqrMag = dir.sqrMagnitude;

            if (dirSqrMag > 0.0001f)
            {
                _lastLookDir = dir.normalized;
            }

            if (_lastLookDir.sqrMagnitude < 0.0001f)
            {
                return;
            }

            float angle = Mathf.Atan2(_lastLookDir.y, _lastLookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

        private IEnumerator CryLoop()
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

        private IEnumerator Death()
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
                IDamageable player = other.gameObject.GetComponent<IDamageable>();
                player?.Damage(damageAmount);
            }
        }

        
    }
}
