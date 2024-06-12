using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]   
public class Enemy : Character
{
    //class variables
    public  static int EnemiesAlive,EnemiesKilled;
    public  static int XpMod = 0;

    public  static Vector2Int HealthRange = new(1, 1);

    //instance variables
    [ShowInInspector]
    private Transform _target;
    private NavMeshAgent _agent = new();


    [BoxGroup("experance")]
    public  int worth;
    
    public  AudioClip[] genericSfx;
    public  AudioClip[] hurtSfx;
    public  AudioClip[] damageSfx;
    
    private Player _player;

    private float _damageCoolDown = 2.00f;
    private bool _canDamage = true;
    
    public static UnityEvent OnLevelUp = new UnityEvent();

    private void Awake()
    {
        //subscribe pause mthods to relavent delgates
        GameManager.Pause += OnPaused;
        GameManager.Unpause += OnResume;

        //setup linkages
        _player = FindObjectOfType<Player>();
        AudioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();

        //setup instances varbales
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = speed;

        _target = _player.transform;
        
        Hp = new(1, 1);
        experance += 1;

    }

    private void OnEnable()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;

        //update game stat
        EnemiesAlive++;

        //play spawn sfx
        StartCoroutine(PlaySound(genericSfx[0]));

        HealthRange.x = Hp.GetMax();
        HealthRange.y = HealthRange.x + 1;

        Hp.SetCurrent(Random.Range(HealthRange.x, HealthRange.y));
        
         OnLevelUp.AddListener(LevelUp);
    }

    private void OnDisable()
    {
        //update game stat
        EnemiesAlive--;
        OnLevelUp.RemoveListener(LevelUp);
        
    }

    private void FixedUpdate()
    {
        if(!GameManager.GamePaused)
        {
            FacePlayer();
        }
        
    }

    private void Update()
    {
        if (!GameManager.GamePaused)
        {
            _agent.SetDestination(_target.position);
        }
        
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
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
    
    private IEnumerator Die()
    {
        GameManager.Score += worth;
        _player.GetExp(experance);

        EnemiesKilled++;
        FindAnyObjectByType<UserInterface>().DisplayKills();
        
        GetComponent<CircleCollider2D>().enabled = false;

        StartCoroutine(PlaySound(hurtSfx[0]));
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        //waite while ending shit is happaening
        yield return new WaitWhile(() => AudioSource.isPlaying);

        gameObject.SetActive(false);
    }

    private void LevelUp()
    {
        level++;
        experance += 1;
        
        HealthRange.x += 1;
        HealthRange.y += 1;
        
        speedMod += 0.5f;
    }

    private void OnPaused()
    {
        //make sure agent is alive and active
        if(_agent != null && _agent.isActiveAndEnabled)
            _agent.isStopped = true;
  
    }
    private void OnResume() 
    {
        if (_agent != null && _agent.isActiveAndEnabled)
            _agent.isStopped = false;
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

    private IEnumerator HitCoolDown()
    {
        yield return new WaitForSeconds(_damageCoolDown);
        _canDamage = true;
    }
}
