using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    public Pistol(GunData data) : base(data)
    {
    }

    public override void Primary(Transform transform,Vector3 spawn,int damagemod)
    {
        if(canFire)
        {
            PlaySound(Fire);
            ammo.Use();

            RaycastHit2D hit = Physics2D.Raycast(spawn, transform.right);

            if(hit.collider == null)
            {
                hit.point = spawn + (transform.right*10);
            }

            var trail = GameObject.Instantiate(BulletTrail,spawn,transform.rotation);
            transform.gameObject.GetComponent<PlayerWeaponHandler>().StartCoroutine(SpawnBullet(trail.GetComponent<TrailRenderer>(),hit,damagemod));

        }else{
            PlaySound(Empty);
        }
        
    }

    public override IEnumerator Reload()
    {
        ammo.setReload(true);
        
        PlaySound(ReloadSFX[0]);

        yield return new WaitWhile(() => isPlaying);

        PlaySound(ReloadSFX[1]);

        yield return new WaitWhile(() => isPlaying);

        ammo.Reload();
    }

    private IEnumerator SpawnBullet(TrailRenderer trail, RaycastHit2D hit2D, int damagemod)
    {
        float time = 0;
        Vector2 startPos = trail.transform.position.WithAxis(Axis.z,1);

        while(time < 1)
        {
            trail.transform.position = Vector2.Lerp(startPos,hit2D.point,time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        //damage target
        if(hit2D.collider != null && hit2D.collider.TryGetComponent<IHittable>(out IHittable target))
        {
            target.Damage(getDamage()+damagemod);
        }

        GameObject.Destroy(trail.gameObject,time);  
    }
}
