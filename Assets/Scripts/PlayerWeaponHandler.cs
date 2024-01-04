using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponHandler : MonoBehaviour
{
    public List<Transform> BulletSpawnLocations = new List<Transform>();


    private Player player;
    private AudioSource audioSource;
    private UserInterface ui
    {
        get { return UserInterface.UI;  }
    }
    private InputActionAsset actions;

    public GameObject BulletTrail;
    
    public AudioClip Fire, Empty;
    public AudioClip[] ReloadSFX;

    public double cooldown;

    public Ammo ammo;

    public Damage damage;

    private bool canFire = true;
    private bool hasAmmo
    {
        get
        {
            if (ammo.getCurrentMag() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private bool isPlaying
    {
        get
        {
            return audioSource.isPlaying;
        }
    }

    public void Initialize(InputActionAsset actionMap,Player p)
    {
        actions = actionMap;
        actions.FindActionMap("Player").Enable();
        actions.FindActionMap("Player").FindAction("Fire").performed += OnFire;
        actions.FindActionMap("Player").FindAction("Reload").performed += OnReload;

        player = p;

        ammo = new(9,2);
        damage = new(2);

    }

    private void OnEnable()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void MagSizeUp(int amt)
    {
       ammo.addToMaxAmmo(amt);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!GameManager.GamePaused)
        {
            Primary(BulletSpawnLocations[0].position);
            ui.UpdateAmmoDisplays(ammo.ToString());
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        StartCoroutine(reload());
    }

    private void Primary( Vector3 spawn)
    {
        if (hasAmmo && canFire)
        {
            canFire = false;
            PlaySound(Fire);
            ammo.Use();

            RaycastHit2D hit = Physics2D.Raycast(spawn, transform.right);

            if (hit.collider == null)
            {
                hit.point = spawn + (transform.right * 10);
            }

            var trail = GameObject.Instantiate(BulletTrail, spawn, transform.rotation);
            transform.gameObject.GetComponent<PlayerWeaponHandler>().StartCoroutine(SpawnBullet(trail.GetComponent<TrailRenderer>(), hit));

            StartCoroutine(fireCooldown());
        }
        else if(!hasAmmo)
        {
            PlaySound(Empty);
        }

    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private IEnumerator reload()
    {
        yield return StartCoroutine(Reload());
        ui.UpdateAmmoDisplays(ammo.ToString());
    }
    private IEnumerator SpawnBullet(TrailRenderer trail, RaycastHit2D hit2D)
    {
        float time = 0;
        Vector2 startPos = trail.transform.position.WithAxis(Axis.z, 1);

        while (time < 1)
        {
            trail.transform.position = Vector2.Lerp(startPos, hit2D.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        //damage target
        if (hit2D.collider != null && hit2D.collider.TryGetComponent<IHittable>(out IHittable target))
        {
            target.Damage(damage.GetDamage());
        }

        GameObject.Destroy(trail.gameObject, time);
    }
    private IEnumerator Reload()
    {
        ammo.setReload(true);

        PlaySound(ReloadSFX[0]);

        yield return new WaitWhile(() => isPlaying);

        PlaySound(ReloadSFX[1]);

        yield return new WaitWhile(() => isPlaying);

        ammo.Reload();
    }
    private IEnumerator fireCooldown() 
    {
        yield return new WaitForSeconds((float)cooldown);
        canFire = true;
    }

}