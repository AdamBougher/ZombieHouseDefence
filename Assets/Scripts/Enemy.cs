using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]   
public class Enemy : Character
{
    //class variables
    public  static int EnemysAlive,EnemysKilled;
    private static int EnemyLevel = 1;
    public static int startingHP = 1;
    public  static Vector2Int healthRange;

    //instance variables
    [ShowInInspector]
    private Transform target;
    private NavMeshAgent agent = new();


    [BoxGroup("experance")]
    public  int Worth;
    private static int exp = 1;
    public  AudioClip[] genericSFX, hurtSFX, damageSFX;

    private Player player;
    private AudioSource audioSource;

    private void Awake()
    {
        //subscribe pause mthods to relavent delgates
        GameManager.ua_Pause += OnPaused;
        GameManager.ua_Unpause += OnResume;

        //setup linkages
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        //setup instances varbales
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;

        target = player.transform;

        exp = EnemyLevel + (EnemyLevel - 1);
        hp = new(startingHP, startingHP);

        healthRange.x = hp.GetMax();
        healthRange.y = healthRange.x + 1;

        hp.SetCurrent(Random.Range(healthRange.x, healthRange.y));
    }

    private void OnEnable()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;

        //update game stat
        EnemysAlive++;

        //play spawn sfx
        StartCoroutine(PlaySound(genericSFX[0]));
    }

    private void OnDisable()
    {
        //update game stat
        EnemysAlive--;
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
            agent.SetDestination(target.position);
        }
        
    }

    public override void Damage(int amt)
    {
        base.Damage(amt);

        //play damage sound effect
        StartCoroutine(PlaySound(hurtSFX[0]));

        if (hp.isEmpty)
        {
            StartCoroutine(Die());
        }
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        Vector3 direction = (player.transform.position - transform.position).normalized;
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
                audioSource.clip = clip;
                audioSource.Play();

                yield return new WaitForSeconds(2f);
            }
        }
    }
    
    private IEnumerator Die()
    {
        GameManager.Score += Worth;
        player.GetEXP(exp);

        EnemysKilled++;
        FindAnyObjectByType<UserInterface>().DisplayKills();

        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        //waite while ending shit is happaening
        yield return new WaitWhile(() => audioSource.isPlaying);

        //Destroy(this.gameObject);
        gameObject.SetActive(false);
    }

    public static void LevelUp()
    {
        speedMod += 0.15f;
        healthRange.x += 5;
        healthRange.y += 5;
        EnemyLevel++;
        exp = exp/2 + EnemyLevel;
    }

    private void OnPaused()
    {
        //make sure agent is alive and active
        if(agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
  
    }
    private void OnResume() 
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }
}
