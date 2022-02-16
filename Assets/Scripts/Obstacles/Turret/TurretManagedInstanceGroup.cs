using Patterns;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretManagedInstanceGroup: ManagedInstanceGroup<TurretBehaviour>
    {
        public GameObject turretPrefab;
        public GameObject bulletPrefab;
        private void Start()
        {
            InstantiateManagedObject(turretPrefab, new Vector3(5, 0.5f, 5), Quaternion.identity);
        }
    }
}