using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]   
public class Enemy : Character
{
    //class variables
    public  static int EnemiesAlive, EnemiesKilled;
    private const float DamageCoolDown = 2.00f;
    
    [BoxGroup("experance"), SerializeField]
    private int worth;
    
    public  AudioClip[] genericSfx;
    public  AudioClip[] hurtSfx;
    public  AudioClip[] damageSfx;
    
    private Player _player;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;
    
    [SerializeField]
    private GameManager _gameManager;
    
    //instance variables
    [ShowInInspector]
    private Transform _target;
    private NavMeshAgent _agent;
    
    private bool _canDamage = true;
    
    //methods

    protected void Awake()
    {
        //subscribe pause methods to relevant delegates
        GameManager.Pause += OnPaused;
        GameManager.Unpause += OnResume;

        //setup linkages
        _player = FindFirstObjectByType<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy: Player not found in scene. Enemy will not function correctly.");
            return;
        }

        if (_gameManager == null)
            _gameManager = FindFirstObjectByType<GameManager>();
        
        AudioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();

        //setup instances variables
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = GetSpeed();

        _target = _player.transform;
        
        worth += 1;

        Hp = new(1, 1);

        GameTime.OnMinuetTick += LevelUp;

    }

    protected virtual void OnEnable()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = true;

        //update game stat
        EnemiesAlive++;

        //play spawn sfx
        StartCoroutine(PlaySound(genericSfx[0]));
        Hp.SetCurrent(Random.Range(1, Hp.GetMax()));
        
    }
    protected virtual void OnDisable()
    {
        GameManager.Pause -= OnPaused;
        GameManager.Unpause -= OnResume;
        GameTime.OnMinuetTick -= LevelUp;
    }

    private void Update() {
        if (GameManager.GamePaused || _player == null)
            return;
        
        _agent.SetDestination(_target.position);
        FacePlayer();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_canDamage) return;

        if (!other.gameObject.TryGetComponent<IHittable>(out var hit)) return;
        
        _canDamage = false;
        hit.Damage(1);
        StartCoroutine(HitCoolDown());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_canDamage) return;
        
        if (!other.gameObject.CompareTag("Destructible")) return;

        if (!other.TryGetComponent<Door>(out var door)) return;
        _canDamage = false;
        door.Damage(1);
        StartCoroutine(HitCoolDown());

    }
    
    private void OnPaused()
    {
        // Ensure the Enemy object and NavMeshAgent are valid before accessing
        if (this == null || _agent == null || !_agent.isActiveAndEnabled) return;

        _agent.isStopped = true;
    }
    private void OnResume() 
    {
        if (this == null || _agent == null || !_agent.isActiveAndEnabled) return;
        _agent.isStopped = false;
    }
    
    public override void Damage(int amt)
    {
        base.Damage(amt);
        
        if (Hp.IsEmpty)
        {
            StartCoroutine(Die());
            return;
        }
        
        //play damage sound effect
        if (hurtSfx != null && hurtSfx.Length > 0)
            StartCoroutine(PlaySound(hurtSfx[0]));
    }

    private void FacePlayer()
    {
        if (_player == null) return;
        var direction = (_player.transform.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new (0, 0, angle);
    }
    
    private IEnumerator Die()
    {
        if (_player != null)
        {
            GameManager.Score += worth;
            _player.GetExp(worth);
        }

        EnemiesKilled++;
        GameManager.UserInterface?.DisplayKills();
        
        _collider.enabled = false;

        if (hurtSfx != null && hurtSfx.Length > 0)
            StartCoroutine(PlaySound(hurtSfx[0]));
        _spriteRenderer.enabled = false;
        //waite while ending shit is happening
        yield return new WaitWhile(() => AudioSource.isPlaying);
        
        EnemyPool.SharedInstance.ReturnToPool(this);
    }

    private void LevelUp(object sender, EventArgs e)
    {
        level++;
        worth += 1;
        
        Hp.IncreaseMax(2);
        
        speedMod += 0.5f;
    }

    private IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(DamageCoolDown);
        _canDamage = true;
    }
    
}
