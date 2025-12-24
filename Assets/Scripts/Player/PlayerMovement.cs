using UnityEngine;
using UnityEngine.InputSystem;

namespace ZombieHouseDefense
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Vector2 _moveInput;
        private Player _player;
        private Collider2D _collider;

        private void Awake()
        {
            if (TryGetComponent<Player>(out var player))
            {
                _player = player;
            }
            _collider = GetComponent<Collider2D>();
        }

        private void OnDisable()
        {
            _moveInput = Vector2.zero; 
        }

        private void Update()
        {
            Move();
        }

        // Called by PlayerInput (Send Messages)
        private void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        private void Move(){
            if (_moveInput.sqrMagnitude > 0.0001f)
            {
                Vector3 delta = new Vector3(_moveInput.x, _moveInput.y, 0f);
                Vector3 newPos = transform.position + _player.Speed * Time.deltaTime * delta;

                // Check for collisions at new position (obstacles and enemies)
                int obstacleLayer = LayerMask.GetMask("Obstacles");
                int enemyLayer = LayerMask.GetMask("Enemy");
                int combinedMask = obstacleLayer | enemyLayer;
                if (!Physics2D.OverlapCircle(newPos, 0.3f, combinedMask))
                {
                    transform.position = newPos;
                }
            }
        }

    }
}