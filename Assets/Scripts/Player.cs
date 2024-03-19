using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : Character
{
    private const int StartingHp = 10;

    public int hpRegenAmt = 0;
    public float hpRegenCooldown = 3f;
    public int luck = 0;
    public int armor = 0;
        
    private Rigidbody2D _rb;
    
    [HideInInspector]
    public PlayerWeaponHandler weaponHandler;

    public InputActionAsset actions;

    private readonly Dictionary<Upgrade, int> _upgrades = new(8);

    private AudioClip _hurtSfx;

    private AudioSource _audioSource;
    
    private bool _interactionCheck = false;

    private bool _isDead = false;

    private void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        UserInterface.OnLoaded += OnUILoad;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start() 
    {
        //subscribe pause to the action
        GameManager.Pause += OnPaused;

        Hp = new CharacterResource(StartingHp, StartingHp);
        
        Hp.SetCurrent(Hp.GetMax());

        StartCoroutine(HpRegen());

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        _audioSource = GetComponent<AudioSource>();
        
        weaponHandler.Initialize(actions, _audioSource);
    }
    
    private void OnUILoad()
    {
        UserInterface.UI.UpdateHp(Hp.GetCurrent());
    }
    
    private void OnMove(InputValue value)
    {
        //if game is not paused or over
        if (!GameManager.GamePaused && !GameManager.GameOver && !_isDead)
        {
            _rb.velocity = value.Get<Vector2>() * (GetSpeed());
        }
    }
    
    private void OnInteract(InputValue value)
    {
        if (_isDead) return;
        
        _interactionCheck = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            Damage(1);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_interactionCheck) return;
        
        
        if (other.TryGetComponent<Door>(out var door))
        {
            door.Enter();
        }
        _interactionCheck = false;
    }

    private void OnFacing(InputValue value)
    {
        if(GameManager.GamePaused) return;
        
        var position = value.Get<Vector2>();
        
        System.Diagnostics.Debug.Assert(Camera.main != null, "Camera.main != null");
        var worldPos = Camera.main.ScreenToWorldPoint(position);
        worldPos.z = 0f;

        var direction = (worldPos - transform.position);

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public override void Damage(int amt)
    {
        if (_isDead) return;
        
        base.Damage(amt);

        if (_audioSource.clip != _hurtSfx) _audioSource.clip = _hurtSfx;
        
        
        _audioSource.Play();
        
        UserInterface.UI.UpdateHp(Hp.GetCurrent());
        
        if (!Hp.IsEmpty) return;
        StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        _isDead = true;
        
        _audioSource.clip = Resources.Load<AudioClip>("Sound/erl");
        
        _audioSource.Play();

        while (_audioSource.isPlaying)
        {
            yield return null;
        }
        
        GameManager.GameOver = true;
        GameManager.GamePaused = true;

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private void OnPaused()
    {
        _rb.velocity = new Vector2();
    }

    /// <summary>
    /// Run Level Up Code
    /// </summary>
    /// <param name="option">what attribute to change</param>
    /// <returns>True is level up option has not ben upgraded before</returns>
    public void LevelUp(Upgrade option)
    {
        //Apply the upgrade by it's name
        Upgrade.UpgradeActions[option.Name](this);

        if (!_upgrades.TryAdd(option, 1))
        {
            _upgrades[option]++;
        }


        UserInterface.UI.imagePanel.AddToUI(option,_upgrades[option]);

    }
    
    public void GetExp(int amt)
    {
        GainExperance(amt,Hp, FindFirstObjectByType<GameManager>());
    }

    private IEnumerator HpRegen()
    {
        while(true)
        {
            yield return new WaitForSeconds(hpRegenCooldown);
            if (hpRegenAmt <= 0) continue;
            
            Hp.IncreaseCurrent(hpRegenAmt);
            UserInterface.UI.UpdateHp(Hp.GetCurrent());

        }
    }
    
}