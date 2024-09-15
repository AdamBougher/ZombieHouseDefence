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
        _player = FindObjectOfType<Player>();
        AudioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();

        //setup instances varbales
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = speed;

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
        //update game stat
        EnemiesAlive--;
        
    }

    private void Update() {
        if (GameManager.GamePaused) 
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
        //make sure agent is alive and active
        if(_agent is not null && _agent.isActiveAndEnabled)
            _agent.isStopped = true;
  
    }
    private void OnResume() 
    {
        if (_agent is not null && _agent.isActiveAndEnabled)
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
        StartCoroutine(PlaySound(hurtSfx[0]));
        
    }

    private void FacePlayer()
    {
        var direction = (_player.transform.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new (0, 0, angle);
    }
    
    private IEnumerator Die()
    {
        GameManager.Score += worth;
        _player.GetExp(worth);

        EnemiesKilled++;
        GameManager.UserInterface.DisplayKills();
        
        _collider.enabled = false;

        StartCoroutine(PlaySound(hurtSfx[0]));
        _spriteRenderer.enabled = false;
        //waite while ending shit is happening
        yield return new WaitWhile(() => AudioSource.isPlaying);
        
        EnemyPool.SharedInstance.ReturnToPool(this.gameObject);
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
