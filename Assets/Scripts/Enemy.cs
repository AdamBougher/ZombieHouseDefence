using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]   
public class Enemy : Character
{
    //class variables
    public  static int EnemiesAlive,EnemiesKilled;
    private static int enemyLevel = 1;
    private static readonly int StartingHp = 1;
    
    public  static Vector2Int HealthRange;

    //instance variables
    [ShowInInspector]
    private Transform _target;
    private NavMeshAgent _agent = new();


    [BoxGroup("experance")]
    public  int worth;
    private static int exp = 1;
    public  AudioClip[] genericSfx;
    public  AudioClip[] hurtSfx;
    public  AudioClip[] damageSfx;

    private Player _player;
    private AudioSource _audioSource;

    private float _damageCoolDown = 2.00f;
    private bool _canDamage = true;

    private void Awake()
    {
        //subscribe pause mthods to relavent delgates
        GameManager.UaPause += OnPaused;
        GameManager.UaUnpause += OnResume;

        //setup linkages
        _player = FindObjectOfType<Player>();
        _audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();

        //setup instances varbales
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = speed;

        _target = _player.transform;

        exp = enemyLevel + (enemyLevel - 1);
        Hp = new(StartingHp, StartingHp);

        HealthRange.x = Hp.GetMax();
        HealthRange.y = HealthRange.x + 1;

        Hp.SetCurrent(Random.Range(HealthRange.x, HealthRange.y));
    }

    private void OnEnable()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;

        //update game stat
        EnemiesAlive++;

        //play spawn sfx
        StartCoroutine(PlaySound(genericSfx[0]));
    }

    private void OnDisable()
    {
        //update game stat
        EnemiesAlive--;
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

        //play damage sound effect
        StartCoroutine(PlaySound(hurtSfx[0]));

        if (Hp.IsEmpty)
        {
            StartCoroutine(Die());
        }
    }

    private void FacePlayer()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<Player>();
        }

        Vector3 direction = (_player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private IEnumerator PlaySound(AudioClip clip) 
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.GamePaused == false);

            yield return new WaitForSeconds(2f);
            float chance = Random.Range(0, 20);
            yield return new WaitUntil(() => GameManager.GamePaused == false);
            if (chance <= 4)
            {
                _audioSource.clip = clip;
                _audioSource.Play();

                yield return new WaitForSeconds(2f);
            }
        }
    }
    
    private IEnumerator Die()
    {
        GameManager.Score += worth;
        _player.GetExp(exp);

        EnemiesKilled++;
        FindAnyObjectByType<UserInterface>().DisplayKills();

        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        //waite while ending shit is happaening
        yield return new WaitWhile(() => _audioSource.isPlaying);

        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }

    public static void LevelUp()
    {
        SpeedMod += 0.15f;
        HealthRange.x += 5;
        HealthRange.y += 5;
        enemyLevel++;
        exp = exp/2 + enemyLevel;
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
