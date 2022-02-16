using MobAI.Anxiety;
using MobAI.NpcCommon;
using Patterns;
using UnityEngine;

namespace MobAI
{
    public class MobManagedInstanceGroup<T>: ManagedInstanceGroup<T>
    where T: BaseNpc<T>
    {
        public override void InstantiateManagedObject(GameObject go, Vector3 position, Quaternion rotation)
        {
            
        }
    }
}