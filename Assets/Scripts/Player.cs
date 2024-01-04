using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Game;

public class Player : Character
{

    public static int startingHP = 10;

    private Rigidbody2D rb;
    [HideInInspector]
    public PlayerWeaponHandler weaponHandler;
    private UserInterface ui;
    // private field to store move action reference
    private InputAction moveAction, facing;

    public InputActionAsset actions;

    private readonly Dictionary<Upgrade, int> upgrades = new(8);


    private void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
        hp.SetCurrent(hp.GetMax());
        
    }

    private void Awake() 
    {
        //setup refrences
        rb = GetComponent<Rigidbody2D>();
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        ui = FindAnyObjectByType<UserInterface>();

        weaponHandler.Initialize(actions,this);

        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = actions.FindActionMap("Player").FindAction("Move");
        facing = actions.FindActionMap("Player").FindAction("Facing");

        //subscripe pause to the action
        GameManager.ua_Pause += OnPaused;

        hp = new(startingHP, startingHP);

    }

    private void FixedUpdate() 
    {
        //if game is not paused or over
        if (!GameManager.GamePaused && !GameManager.GameOver)
        {
            Utility.LookAt(facing.ReadValue<Vector2>(), transform);

            rb.velocity = moveAction.ReadValue<Vector2>() * (getSpeed());
        }
        
    }

    public override void Damage(int amt)
    {
        base.Damage(amt);
        if(hp.isEmpty)
        {
            Debug.Log("You died!");
            GameManager.GameOver = true;
            GameManager.GamePaused = true;
        }
    }

    public void OnPaused()
    {
        rb.velocity = new();
    }

    /// <summary>
    /// Run Level Up Code
    /// </summary>
    /// <param name="option">what atribute to change</param>
    /// <returns>True is level up option has not ben upgraded before</returns>
    public bool LevelUP(Upgrade option)
    {
        Upgrade.ApplyUpgrade(this,option);

        if(upgrades.ContainsKey(option))
        {
            upgrades[option]++;
            return false;
        }else{
            upgrades.Add(option, 1);
            return true;
        }

    }
    

    public void GetEXP(int amt)
    {
        GainExperance(amt,hp, FindFirstObjectByType<GameManager>());
    }

}