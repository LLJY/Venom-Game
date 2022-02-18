using Patterns;
using UnityEngine;

namespace Obstacles.Turret
{
    public class BulletObjectPool : ObjectPool<Bullet>
    {
        public void ShootBullet(Transform barrel, float speed, Quaternion rotation)
        {
            var bullet = TakeFromPool();
            var bulletTransform = bullet.transform;
            bulletTransform.position = barrel.position + (barrel.forward);
            bulletTransform.rotation = rotation;
            var rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.AddForce(bullet.transform.forward * speed,
                ForceMode.VelocityChange);
        }
    }
}