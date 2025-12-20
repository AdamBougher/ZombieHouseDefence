using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [BoxGroup("TextElements")]
    public TMP_Text ammo, clock, kills, level, hp, jumbotron;
    [BoxGroup("ObjectElements")]
    public GameObject menu, levelUpMenu, itemUI;
    public Image xpBar;
    public List<UpgradeChoice> levelUpOption;
    public UIItemDisplay imagePanel;

    [BoxGroup("References")]
    [SerializeField]
    private Player _player;
    
    [BoxGroup("References")]
    [SerializeField]
    private PlayerWeaponHandler _weaponHandler;

    public static UserInterface UI { get; private set; }
    public static event Action OnLoaded;

    private const int   JumbotronOnScreenTime = 3;
    private const float JumbotronFadeSpeed    = 2f; // alpha units per second
    private const float ClockUpdateInterval   = 0.5f;

    private void Awake()
    {
        UI = this;
        clock.SetText("00:00");
        StartCoroutine(UpdateClockLoop());
        OnLoaded?.Invoke();
        StartCoroutine(TutorialRoutine());
    }

    private void OnEnable()
    {
        // Get player reference if not serialized
        if (_player == null)
            _player = FindFirstObjectByType<Player>();
        
        if (_player != null)
        {
            _player.OnHpChanged += UpdateHp;
            _player.OnXpPercentageChanged += UpdateXpBar;
        }
        else
        {
            Debug.LogWarning("UserInterface.OnEnable: Player reference not found.");
        }

        // Get weapon handler reference if not serialized
        if (_weaponHandler == null)
            _weaponHandler = FindFirstObjectByType<PlayerWeaponHandler>();
        
        if (_weaponHandler != null)
        {
            _weaponHandler.OnAmmoChanged += UpdateAmmoDisplays;
        }
        else
        {
            Debug.LogWarning("UserInterface.OnEnable: PlayerWeaponHandler reference not found.");
        }

        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from gameplay events
        if (_player != null)
        {
            _player.OnHpChanged -= UpdateHp;
            _player.OnXpPercentageChanged -= UpdateXpBar;
        }

        if (_weaponHandler != null)
        {
            _weaponHandler.OnAmmoChanged -= UpdateAmmoDisplays;
        }

        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    private void HandlePauseStateChanged()
    {
        // Optional: handle pause-state-dependent UI logic here
        // For now, this is a placeholder for future pause UI updates
    }

    private IEnumerator TutorialRoutine()
    {
        ShowJumbotron("Use W,A,S and D to move around");
        yield return new WaitForSeconds(6.5f);
        ShowJumbotron("Aim with the Mouse!");
    }

    private IEnumerator UpdateClockLoop()
    {
        while (!GameManager.GameOver)
        {
            yield return new WaitForSeconds(ClockUpdateInterval);
            clock.SetText(GameManager.Time.ToString());
        }
    }

    public void UpdateAmmoDisplays(string str)
    {
        if (ammo != null)
            ammo.SetText(str);
    }

    public void UpdateXpBar(float xpAmt)
    {
        if (xpBar != null)
            xpBar.fillAmount = xpAmt;
    }

    public void DisplayKills()
    {
        if (kills != null)
            kills.SetText(Enemy.EnemiesKilled.ToString());
    }

    public void UpdateLevel(string lvl)
    {
        if (level != null)
            level.SetText($"Lvl: {lvl}");
    }

    public void UpdateHp(int amt)
    {
        if (hp != null)
            hp.SetText(amt.ToString());
    }

    public void ReturnToMenu()
    {
        StartCoroutine(SceneLoadingRoutine());
    }

    private IEnumerator SceneLoadingRoutine()
    {
        var loadOp = SceneManager.LoadSceneAsync("MainMenu");
        yield return new WaitUntil(() => loadOp.isDone);

        var unloadOp = SceneManager.UnloadSceneAsync("MainMenu");
        yield return new WaitUntil(() => unloadOp.isDone);
    }

    public void ShowJumbotron(string text)
    {
        if (jumbotron != null)
            StartCoroutine(JumbotronRoutine(text));
        else
            Debug.LogWarning("UserInterface.ShowJumbotron: Jumbotron TextMeshProUGUI is null.");
    }

    private IEnumerator JumbotronRoutine(string text)
    {
        if (jumbotron == null)
            yield break;
            
        jumbotron.SetText(text);
        var col = jumbotron.color;

        // Fade in
        while (jumbotron.color.a < 1f)
        {
            col.a = Mathf.Min(1f, col.a + JumbotronFadeSpeed * Time.deltaTime);
            jumbotron.color = col;
            yield return null;
        }

        yield return new WaitForSeconds(JumbotronOnScreenTime);

        // Fade out
        while (jumbotron.color.a > 0f)
        {
            col.a = Mathf.Max(0f, col.a - JumbotronFadeSpeed * Time.deltaTime);
            jumbotron.color = col;
            yield return null;
        }
    }
}
