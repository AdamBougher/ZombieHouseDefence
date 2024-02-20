using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
public class Player : Character
{
    private const int StartingHp = 10;

    private Rigidbody2D _rb;
    
    [HideInInspector]
    public PlayerWeaponHandler weaponHandler;

    public InputActionAsset actions;

    private readonly Dictionary<Upgrade, int> _upgrades = new(8);

    [SerializeField]
    private AudioClip hurtsfx;

    private AudioSource _audioSource;

    private void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
        Hp.SetCurrent(Hp.GetMax());

        UserInterface.OnLoaded += OnUILoad;

    }

    private void Awake() 
    {
        
        //setup references
        _rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();

        weaponHandler.Initialize(actions);
        
        //subscribe pause to the action
        GameManager.UaPause += OnPaused;

        Hp = new CharacterResource(StartingHp, StartingHp);

        _audioSource = GetComponent<AudioSource>();

    }
    
    private void OnUILoad()
    {
        UserInterface.UI.UpdateHp(Hp.GetCurrent());
    }
    
    private void OnMove(InputValue value)
    {
        //if game is not paused or over
        if (!GameManager.GamePaused && !GameManager.GameOver)
        {
            _rb.velocity = value.Get<Vector2>() * (GetSpeed());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            Damage(1);
        }
    }

    private void OnFacing(InputValue value)
    {
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
        base.Damage(amt);

        if (_audioSource.clip != hurtsfx) _audioSource.clip = hurtsfx;
        
        
        _audioSource.Play();
        
        UserInterface.UI.UpdateHp(Hp.GetCurrent());
        
        if (!Hp.IsEmpty) return;

        gameObject.SetActive(false);
        
        GameManager.GameOver = true;
        GameManager.GamePaused = true;
    }

    private void OnPaused()
    {
        _rb.velocity = new();
    }

    /// <summary>
    /// Run Level Up Code
    /// </summary>
    /// <param name="option">what attribute to change</param>
    /// <returns>True is level up option has not ben upgraded before</returns>
    public bool LevelUp(Upgrade option)
    {
        Upgrade.ApplyUpgrade(this,option);

        if(!_upgrades.TryAdd(option, 1))
        {
            _upgrades[option]++;
            return false;
        }else{
            return true;
        }

    }
    

    public void GetExp(int amt)
    {
        GainExperance(amt,Hp, FindFirstObjectByType<GameManager>());
    }

}