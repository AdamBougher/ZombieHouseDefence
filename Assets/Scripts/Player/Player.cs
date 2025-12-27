using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using ZombieHouseDefense.Interfaces;


namespace ZombieHouseDefense.Player
{
    public class Player : Core.Character, IDamageable
    {
        private Vector2 _lookInput;
        private Vector2 _lastLookDir = Vector2.right;

        [SerializeField] private float _interactionRange = 2f;
        [SerializeField] private int IFrameTime = 1;
        [SerializeField] private float _flashFrequency = 0.1f;
        [SerializeField, ReadOnly] private bool _isInvincible = false;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer armsRenderer;

        [SerializeField] private bool _canInteract = true;
        [SerializeField] private float _interactionCooldown = 0.5f;

        private void Start()
        {
            Hp = new CharacterResource(10, 10, false);
        }

        // Called by PlayerInput
        private void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
            // no camera, cannot resolve screen position
            Camera cam = Camera.main;

            // If input is from mouse (screen position), magnitude will typically be > 1
            if (_lookInput.sqrMagnitude > 1f)
            {
                if (cam == null) return;

                float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
                Vector3 world = cam.ScreenToWorldPoint(new Vector3(_lookInput.x, _lookInput.y, z));
                Vector2 dir = (Vector2)(world - transform.position);

                if (dir.sqrMagnitude > 0.0001f)
                {
                    _lastLookDir = dir.normalized;
                }
            }
            // If input is from right stick (already a direction)
            else if (_lookInput.sqrMagnitude > 0.0001f)
            {
                _lastLookDir = _lookInput.normalized;
            }

            if (_lastLookDir.sqrMagnitude < 0.0001f) return;

            float angle = Mathf.Atan2(_lastLookDir.y, _lastLookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnInteract(InputValue value)
        {
            if(!_canInteract) return;
            _canInteract = false;
            Debug.Log("Player Interact pressed");
            // Find all interactable objects in range
            Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, _interactionRange);

            var interactables = new HashSet<IInteractable>();
            foreach (Collider2D collider in collidersInRange)
            {
                if (collider == null) continue;

                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    interactables.Add(interactable);
                    continue;
                }

                // If the collider is on a child object, prefer the parent interactable.
                interactable = collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    interactables.Add(interactable);
                }
            }

            foreach (IInteractable interactable in interactables)
            {
                interactable.Interact();
            }

            StartCoroutine(InteractCooldown());
        }

        private IEnumerator InteractCooldown()
        {
            yield return new WaitForSeconds(_interactionCooldown);
            _canInteract = true;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Player collided with " + collision.gameObject.name);
        }

        protected override void Die(){
            Debug.Log($"{gameObject.name} (Player) has died.");
        }

        public override void Damage(int amt)
        {
            if (_isInvincible) return;

            Hp.Current -= amt;

            if (Hp.IsEmpty)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityFrames());
            }
        }

        private IEnumerator InvincibilityFrames()
        {
            _isInvincible = true;
            float elapsedTime = 0f;

            while (elapsedTime < IFrameTime)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }

                if (armsRenderer != null)
                {
                    armsRenderer.enabled = !armsRenderer.enabled;
                }

                yield return new WaitForSeconds(_flashFrequency);
                elapsedTime += _flashFrequency;
            }

            // Ensure sprite is visible at the end
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
            if (armsRenderer != null)
            {
                armsRenderer.enabled = true;
            }
            _isInvincible = false;
        }

        public static Transform GetTransform()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                return player.transform;
            }
            return null;
        }

    }
}