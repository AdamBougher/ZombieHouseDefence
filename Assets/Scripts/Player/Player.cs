using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public event Action<bool> OnBuildModeChanged;
    public event System.Action<int> OnHpChanged;
    public event System.Action<float> OnXpPercentageChanged;

    private const int StartingHp = 10;

    [Header("Player Stats")]
    public int hpRegenAmt = 0, luck = 0, armor = 0;
    public float hpRegenCooldown = 3f;

    [Header("Experience")]
    [SerializeField] private int experance, nextLevel;
    private float ExpPercentage => experance / (float)nextLevel;

    [Header("Mode")]
    [SerializeField] private bool buildMode = false;

    [SerializeField] private InputActionAsset actions;
    private Rigidbody2D _rb;
    private AudioClip _hurtSfx;
    [SerializeField] private AudioClip deathClip;

    [HideInInspector] public PlayerWeaponHandler weaponHandler;
    [HideInInspector] public PlayerBuilding buildingHandler;
        [SerializeField] private PlayerArmsManager armsManager;

    private bool _interactionCheck = false, _isDead = false;
    public bool levelingUp = false;

    private Vector2 _lastInput;
    private System.Random random = new();

    private void OnEnable()
    {
        // Enable input actions
        actions.FindActionMap("Player")?.Enable();

        // Subscribe to events
        GameManager.Pause += OnPaused;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UserInterface.OnLoaded += OnUILoad;

        // Initialize HP
        Hp = new CharacterResource(StartingHp, StartingHp);
        Hp.SetCurrent(Hp.GetMax());

        // Start HP regeneration
        StartCoroutine(HpRegen());
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        GameManager.Pause -= OnPaused;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UserInterface.OnLoaded -= OnUILoad;

        // Disable input actions
        actions.FindActionMap("Player")?.Disable();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Initialize references
        _rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        buildingHandler = GetComponentInChildren<PlayerBuilding>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void OnUILoad()
    {
        // Notify UI of current HP via event
        OnHpChanged?.Invoke(Hp.GetCurrent());
    }

    private void OnMove(InputValue value)
    {
        if (GameManager.GamePaused || GameManager.GameOver || _isDead) return;

        // Move the player
        _rb.linearVelocity = value.Get<Vector2>() * GetSpeed();
    }

    private void OnFacing(InputValue value)
    {
        if (GameManager.GamePaused) return;

        var position = value.Get<Vector2>();

        // Handle mouse or joystick input
        if (position.magnitude > 1)
        {
            if (Camera.main == null) return;
            var worldPos = Camera.main.ScreenToWorldPoint(position);
            worldPos.z = 0f;
            position = (worldPos - transform.position).normalized;
        }
        else if (position.magnitude > 0)
        {
            _lastInput = position;
        }
        else
        {
            position = _lastInput;
        }

        // Rotate the player
        var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnFire(InputValue value)
    {
        if (GameManager.GamePaused) return;

        if (buildMode)
        {
            buildingHandler.Place();
        }
        else
        {
            weaponHandler.Primary();
        }
    }

    public void OnReload(InputValue value)
    {
        if (buildMode) return;

        weaponHandler.StartReload();
    }

    private void OnSwitchHeld(InputValue value)
    {
        buildMode = !buildMode;
        OnBuildModeChanged?.Invoke(buildMode);

        if (buildMode)
        {
            buildingHandler.SetArms();
            buildingHandler.currentPlacement.gameObject.SetActive(true);
        }
        else
        {
            weaponHandler.SetArms();
            buildingHandler.currentPlacement.gameObject.SetActive(false);
        }

        // Delegate arms sprite update to PlayerArmsManager
        if (armsManager != null)
            armsManager.SetArmsSprite(buildMode ? buildingHandler.toolSprite : weaponHandler.weaponSprite);
    }

    private void OnSwapBuild(InputValue value)
    {
        buildingHandler.ChangeItem(value.Get<float>());
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

    public override void Damage(int amt)
    {
        if (_isDead) return;

        if (random.Next(1, 100) <= luck) return;

        base.Damage(amt);

        if (AudioSource.clip != _hurtSfx) AudioSource.clip = _hurtSfx;
        AudioSource.Play();

        OnHpChanged?.Invoke(Hp.GetCurrent());

        if (Hp.IsEmpty)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        _isDead = true;

        if (AudioSource != null && deathClip != null)
        {
            AudioSource.clip = deathClip;
            AudioSource.Play();
        }

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
        if (_rb == null) return;

        _rb.linearVelocity = Vector2.zero;
    }

    public void GetExp(int amt)
    {
        GainExperience(amt);
    }

    private void GainExperience(int amt)
    {
        experance += amt;
        OnXpPercentageChanged?.Invoke(ExpPercentage);

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

            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.SetupLevelUp();

            yield return new WaitWhile(() => levelingUp);
        }

        OnXpPercentageChanged?.Invoke(ExpPercentage);
    }

    private IEnumerator HpRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(hpRegenCooldown);

            if (hpRegenAmt > 0)
            {
                Hp.IncreaseCurrent(hpRegenAmt);
                OnHpChanged?.Invoke(Hp.GetCurrent());
            }
        }
    }
}