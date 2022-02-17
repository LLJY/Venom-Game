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
            InstantiateManagedObject(teslaCoilPrefab, new Vector3(pos.x, 1.81f, pos.z), Quaternion.Euler(new Vector3(-180f, 0, 0)));
        }
    }
}