using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Player Player;
    public static GameTime time;
    public static UnityAction ua_Pause, ua_Unpause;

    public static bool GameOver = false;
    public static bool GamePaused = false;
    public static int Score;

    //instance variabels
    private UserInterface userInterface
    {
        get { return UserInterface.UI; }
    }
    private AudioSource audioSource;
    public InputActionAsset actions;

    //audio clipsssssssssssssssss
    public AudioClip levelUp;

    private void OnEnable()
    {
        actions.FindActionMap("Game").Enable();
    }

    void Start()
    {
        //Application.targetFrameRate = 240;

        SceneManager.LoadScene("Ui", LoadSceneMode.Additive);

        //setup refrences
        Player          = FindAnyObjectByType<Player>();
        audioSource = GetComponent<AudioSource>();

        //setup input actions
        actions.FindActionMap("Game").FindAction("Menu").performed += OnPause;

        //create and start the game time
        time = new();
        StartCoroutine(time.Time());
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
            ua_Pause.Invoke();
        }
        else
        {
            GamePaused = false;
            ua_Unpause.Invoke();
        }
        
        time.ToggleTimeStopped();
        switch (context)
        {
            case "Menu": userInterface.menu.SetActive(GamePaused); break;
            case "Level": SetupLevelUp(); break;
            default: break;
        }
    }

    private void SetupLevelUp()
    {
        //play audio cue
        audioSource.clip = levelUp;
        audioSource.Play();

        //initlize upgrades
        Stack<Upgrade> temp = Upgrade.GetUpgrades(3);

        Random.InitState(System.DateTime.Now.Millisecond + (int)System.DateTime.Now.Ticks + 420);

        foreach (UpgradeChoice item in userInterface.levelUpOption)
        {

            item.Setup(temp.Pop());

        }
        //show levelup menu
        userInterface.levelUpMenu.SetActive(true);
    }

    public void LevelUp(Upgrade option)
    {
        if(Player.LevelUP(option))
        {
            userInterface.imagePannel.AddToUI(option.Icon);
        }
        userInterface.levelUpMenu.SetActive(false);

        
        Pause();
    }

    

}