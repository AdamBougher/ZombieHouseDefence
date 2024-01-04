using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Player : Character
{

    public static int startingHP = 10;

    private Rigidbody2D rb;
    [HideInInspector]
    public PlayerWeaponHandler weaponHandler;
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
            Transform trans = this.transform;
            LookAt(facing.ReadValue<Vector2>(), this.transform);

            rb.velocity = moveAction.ReadValue<Vector2>() * (getSpeed());
        }
        
    }

    /// <summary>
    /// rotate gameobject transform to be looking twords position
    /// </summary>
    /// <param name="position">position to look at</param>
    /// <param name="transform">calling object's transform</param>
    public void LookAt(Vector2 position, Transform transform)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
        worldPos.z = 0f;

        Vector3 direction = (worldPos - transform.position);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
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