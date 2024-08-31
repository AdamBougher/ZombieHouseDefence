using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class Player : Character
{
    private const int StartingHp = 10;

    public int hpRegenAmt = 0, 
        luck = 0,
        armor = 0;
    private bool _interactionCheck = false, _isDead = false;
    public bool levelingUp = false;
    public float hpRegenCooldown = 3f;
    [BoxGroup("experance")]
    private float ExpPercentage => (float)experance / (float)nextLevel;
        
    private Rigidbody2D _rb;
    
    [HideInInspector]
    public PlayerWeaponHandler weaponHandler;

    public InputActionAsset actions;

    private AudioClip _hurtSfx;
    private Vector2 _lastInput;
    
    private System.Random random = new();
    
    [BoxGroup("experance"),SerializeField]
    protected int experance, nextLevel;
    
    private void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
        
        //subscribe pause to the action
        GameManager.Pause += OnPaused;

        Hp = new CharacterResource(StartingHp, StartingHp);
        
        Hp.SetCurrent(Hp.GetMax());

        StartCoroutine(HpRegen());
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        UserInterface.OnLoaded += OnUILoad;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        AudioSource = GetComponent<AudioSource>();
        
        weaponHandler.Initialize(actions, AudioSource);
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

        switch (position.magnitude)
        {
            // Check if the input is likely a mouse position
            case > 1:
            {
                // Convert from screen space to world space
                var worldPos = Camera.main.ScreenToWorldPoint(position);
                worldPos.z = 0f;
                position = (worldPos - transform.position).normalized;
                break;
            }
            case > 0:
                // If the joystick is being moved, store the input
                _lastInput = position;
                break;
            default:
                // If the joystick is released, use the last non-zero input
                position = _lastInput;
                break;
        }

        var direction = position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public override void Damage(int amt)
    {
        if (_isDead) return;
        var randomNumber = random.Next(1, 100);
        
        if(randomNumber <= luck)
        {
            return;
        }
        
        base.Damage(amt);

        if (AudioSource.clip != _hurtSfx) AudioSource.clip = _hurtSfx;
        
        AudioSource.Play();
        
        UserInterface.UI.UpdateHp(Hp.GetCurrent());
        
        if (!Hp.IsEmpty) return;
        StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        _isDead = true;
        
        AudioSource.clip = Resources.Load<AudioClip>("Sound/erl");
        
        AudioSource.Play();

        while (AudioSource.isPlaying)
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
    
    public void GetExp(int amt)
    {
        GainExperance(amt);
    }

    private void GainExperance(int amt)
    {
        experance += amt;
        UserInterface.UI.xpBar.fillAmount = ExpPercentage;
        
        if (experance >= nextLevel)
        {
            StartCoroutine(LevelUp());
        }
    }

    private IEnumerator LevelUp()
    {
        
        Hp.SetCurrentToMax();
        while (experance >= nextLevel)
        {
            level++;
            experance -= nextLevel;
            nextLevel += 3;
            levelingUp = true;
            
            FindFirstObjectByType<GameManager>().SetupLevelUp();
            
            yield return new WaitWhile( () => levelingUp);
        }
        UserInterface.UI.Updatelevel(level.ToString());
        UserInterface.UI.xpBar.fillAmount = ExpPercentage;
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