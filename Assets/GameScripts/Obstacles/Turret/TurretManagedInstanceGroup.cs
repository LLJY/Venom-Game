using Patterns;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretManagedInstanceGroup: ManagedInstanceGroup<TurretBehaviour>
    {
        public GameObject turretPrefab;
        public GameObject bulletPrefab;
        public void Instantiate(Vector3 pos)
        {
            InstantiateManagedObject(turretPrefab, pos, Quaternion.identity);
        }
    }
}