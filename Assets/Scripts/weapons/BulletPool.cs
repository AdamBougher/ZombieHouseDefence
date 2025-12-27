using UnityEngine;

namespace ZombieHouseDefense.Weapon
{
    public class BulletPool : ObjectPool<Bullet>
    {
        // Convenience static accessors aligning with base generic singleton
        public static BulletPool SharedInstance => ObjectPool<Bullet>.SharedInstance as BulletPool;
        public static BulletPool Instance => SharedInstance;

        // Convenience: get a bullet, set parent/transform, and activate it
        public Bullet GetBullet(Transform parent, Vector3 position, Quaternion rotation)
        {
            var bullet = GetPooledObject();
            if (bullet == null)
            {
                Debug.LogError("BulletPool: objectToPool not set or pool missing.");
                return null;
            }

            if (parent != null)
            {
                bullet.transform.SetParent(parent, false);
            }
            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        // Simpler overload if you only need to parent
        public Bullet GetBullet(Transform parent)
        {
            var bullet = GetPooledObject();
            if (bullet == null)
            {
                Debug.LogError("BulletPool: objectToPool not set or pool missing.");
                return null;
            }
            if (parent != null)
            {
                bullet.transform.SetParent(parent, false);
            }
            bullet.gameObject.SetActive(true);
            return bullet;
        }
    }
}

