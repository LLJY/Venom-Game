using System;
using Patterns;
using UnityEngine;

namespace Obstacles.TeslaCoil
{
    public class TeslaCoilManagedInstanceGroup: ManagedInstanceGroup<TeslaCoil>
    {
        public GameObject teslaCoilPrefab;

        public void Instantiate(Vector3 pos)
        {
            InstantiateManagedObject(teslaCoilPrefab, pos, Quaternion.Euler(new Vector3(-180f, 1.91f, 0)));
        }
    }
}