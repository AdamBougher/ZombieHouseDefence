using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static Player player;
    public static GameTime Time;
    public static UnityAction UaPause, UaUnpause;

    public static bool GameOver = false;
    public static bool GamePaused = false;
    public static int Score;

    //instance variables
    private static UserInterface UserInterface => UserInterface.UI;
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

        //setup refrences
        player          = FindAnyObjectByType<Player>();
        _audioSource = GetComponent<AudioSource>();

        //setup input actions
        actions.FindActionMap("Game").FindAction("Menu").performed += OnPause;

        //create and start the game time
        Time = new GameTime();
        StartCoroutine(Time.Time());
    }

    public static void AddToScore(int amt)
    {
        Score += amt;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        Pause(context.action.name);
    }

    /// <summary>
    /// Toggle wether the game is puaseed or not
    /// </summary>
    /// <param name="context">string of menu to activate</param>
    public void Pause(string context = "")
    {
        if(!GamePaused)
        {
            GamePaused = true;
            UaPause.Invoke();
        }
        else
        {
            GamePaused = false;
            UaUnpause.Invoke();
        }
        
        Time.ToggleTimeStopped();
        switch (context)
        {
            case "Menu": UserInterface.menu.SetActive(GamePaused); break;
            case "Level": SetupLevelUp(); break;
            default: break;
        }
    }

    private void SetupLevelUp()
    {
        //play audio cue
        _audioSource.clip = levelUp;
        _audioSource.Play();

        //initlize upgrades
        Stack<Upgrade> temp = Upgrade.GetUpgrades(3);

        Random.InitState(System.DateTime.Now.Millisecond + (int)System.DateTime.Now.Ticks + 420);

        foreach (UpgradeChoice item in UserInterface.levelUpOption)
        {

            item.Setup(temp.Pop());

        }
        //show levelup menu
        UserInterface.levelUpMenu.SetActive(true);
    }

    public void LevelUp(Upgrade option)
    {
        if(player.LevelUp(option))
        {
            UserInterface.UI.imagePanel.AddToUI(option.Icon);
        }
        UserInterface.levelUpMenu.SetActive(false);


        Pause();
    }

    

}