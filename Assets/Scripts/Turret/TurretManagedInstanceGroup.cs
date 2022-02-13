using System;
using Patterns;
using UnityEngine;

namespace Turret
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