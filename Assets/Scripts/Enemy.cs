using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CircleCollider2D))]
public class Enemy : Character, IHittable
{
    //class variables
    public  static int EnemysAlive,EnemysKilled;
    private static int EnemyLevel = 1;
    public static int startingHP = 1;
    public  static Vector2Int healthRange;


    //instance variables
    [BoxGroup("experance")]
    public  int Worth;
    private static int exp = 1;
    public  AudioClip[] genericSFX, hurtSFX, damageSFX;

    private AIPath AIPath;
    private Player player;
    private AudioSource audioSource;

    public override void Damage(int amt)
    {
        base.Damage(amt);
        //play damage sound effect
        audioSource.clip = hurtSFX[0];
        audioSource.Play();

        if(hp.isEmpty)
        {
            StartCoroutine(Die());
        }
    }

    private void Start()
    {
        GameManager.ua_Pause   += OnPaused;
        GameManager.ua_Unpause += OnResume;

        AIPath = GetComponent<AIPath>();
        player = FindObjectOfType<Player>();
        GetComponent<AIDestinationSetter>().target = player.transform;
        audioSource = GetComponent<AudioSource>();

        EnemysAlive++;

        AIPath.maxSpeed = (getSpeed());

        exp = EnemyLevel + (EnemyLevel-1);

        hp = new(startingHP, startingHP);

        healthRange.x = hp.GetMax();
        healthRange.y = healthRange.x + 1;

        hp.SetCurrent(Random.Range(healthRange.x, healthRange.y));


        StartCoroutine(PlaySound());
    }
    void OnDestroy()
    {
        EnemysAlive  --;
    }

    private void FixedUpdate()
    {
        if(!GameManager.GamePaused)
        {
            FacePlayer();
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

    private IEnumerator PlaySound() 
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.GamePaused == false);

            yield return new WaitForSeconds(2f);
            float chance = Random.Range(0, 20);
            yield return new WaitUntil(() => GameManager.GamePaused == false);
            if (chance <= 4)
            {
                audioSource.clip = genericSFX[0];
                audioSource.Play();

                yield return new WaitForSeconds(2f);
            }
        }
    }
    
    private IEnumerator Die()
    {
        GameManager.Score += Worth;
        FindAnyObjectByType<Player>().GetEXP(exp);
        gameObject.SetActive(false);

        //waite while ending shit is happaening
        yield return new WaitWhile(() => audioSource.isPlaying);

        EnemysKilled++;
        FindAnyObjectByType<UserInterface>().DisplayKills();
        Destroy(gameObject);
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
        AIPath.canMove = false;
  
    }
    private void OnResume() 
    {
        AIPath.canMove = true;
    }
}
