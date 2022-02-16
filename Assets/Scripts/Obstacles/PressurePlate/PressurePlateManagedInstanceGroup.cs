using Patterns;
using UnityEngine;

namespace Obstacles.PressurePlate
{
    public class PressurePlateManagedInstanceGroup: ManagedInstanceGroup<PressurePlate>
    {
        public GameObject pressurePlate;
        public void Instantiate(Vector3 pos)
        {
            InstantiateManagedObject(pressurePlate, pos, Quaternion.identity);
        }
    }
}