using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static Player player;
    public static GameTime Time;
    public static UnityAction Pause, Unpause;

    [ShowInInspector]
    public static bool GameOver, GamePaused, CanLevelUp = true;
    public static int Score;

    //instance variables
    public static UserInterface UserInterface => UserInterface.UI;
    public UpgradeManager upgradeManager;
    private AudioSource _audioSource;
    public InputActionAsset actions;

    //audio clips
    public AudioClip levelUp;

    private void OnEnable()
    {
        actions.FindActionMap("Game").Enable();
    }

    private void Start()
    {
        //Application.targetFrameRate = 240;

        SceneManager.LoadScene($"Ui", LoadSceneMode.Additive);

        //setup references
        player       = FindAnyObjectByType<Player>();
        _audioSource = GetComponent<AudioSource>();

        //setup input actions
        actions.FindActionMap("Game").FindAction("Menu").performed += OnPause;

        //create and start the game time
        Time = new GameTime();
        StartCoroutine(Time.Time());
        
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        
        GamePaused = false;
        GameOver = false;
    }

    public static void AddToScore(int amt)
    {
        Score += amt;
    }

    private static void OnPause(InputAction.CallbackContext context)
    {
        PauseGame();
        if(context.action.name == "Menu")
        {
            UserInterface.menu.SetActive(GamePaused);
        }
    }

    /// <summary>
    /// Toggle weather the game is paused or not
    /// </summary>
    /// <param upgradeName="context">string of menu to activate</param>
    /// <param name="context"></param>
    private static void PauseGame(string context = "")
    {
        if (!GamePaused)
        {
            GamePaused = true;

            // Clean up invalid subscriptions before invoking Pause
            if (Pause != null)
            {
                foreach (var d in Pause.GetInvocationList())
                {
                    if (d.Target == null)
                        Pause -= (UnityAction)d;
                }

                Pause.Invoke();
            }
        }
        else
        {
            GamePaused = false;

            // Clean up invalid subscriptions before invoking Unpause
            if (Unpause != null)
            {
                foreach (var d in Unpause.GetInvocationList())
                {
                    if (d.Target == null)
                        Unpause -= (UnityAction)d;
                }

                Unpause.Invoke();
            }
        }

        Time.ToggleTimeStopped();
    }

    public void SetupLevelUp()
    {
        PauseGame();
        //play audio cue
        _audioSource.clip = levelUp;
        _audioSource.Play();
        
        upgradeManager.LevelUpMenuSetup(UserInterface.UI);
       
    }

    public static void EndLevelUp()
    {
        UserInterface.levelUpMenu.SetActive(false);
        PauseGame();
        player.levelingUp = false;
    }  

    private void OnDisable()
    {
        GameManager.Pause -= OnPaused;
        GameManager.Unpause -= OnResume;
    }

    private void OnPaused()
    {

    }

    private void OnResume()
    {
        // Handle resume logic here
    }
}