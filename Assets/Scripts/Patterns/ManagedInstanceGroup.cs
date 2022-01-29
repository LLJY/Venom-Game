using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    /// <summary>
    /// https://blog.unity.com/technology/1k-update-calls
    /// The idea of this script is that mobs, turrets and things of repetitive nature can be managed in a single grouped update
    /// This alleviates the 10k updates problem that the StatefulMB pattern can potentially create.
    /// The limitation is that a Managed Instance Group can only manage one StatefulMB, this is a small optimization as I know
    /// that my code will never have more than 1 managed script at a time.
    /// </summary>
    /// <typeparam name="T">The StatefulMB's type</typeparam>
    public abstract class ManagedInstanceGroup<T>: MonoBehaviour
    
    where T: StatefulMonoBehaviour<T>
    {
        public Dictionary<int, StatefulMonoBehaviour<T>> ManagedBehaviours = new Dictionary<int, StatefulMonoBehaviour<T>>();
        

        protected virtual void Update()
        {
            foreach (var behaviour in ManagedBehaviours)
            {
                behaviour.Value.ManagedUpdate();
            }
        }
        protected virtual void LateUpdate()
        {
            foreach (var behaviour in ManagedBehaviours)
            {
                behaviour.Value.ManagedLateUpdate();
            }
        }
        protected virtual void FixedUpdate()
        {
            foreach (var behaviour in ManagedBehaviours)
            {
                behaviour.Value.ManagedFixedUpdate();
            }
        }

        protected void InstantiateManagedObject(GameObject go, Vector3 position, Quaternion rotation)
        {
            var newObject = GameObject.Instantiate(go, position, rotation);
            ManagedBehaviours.Add(newObject.GetInstanceID(), newObject.GetComponent<StatefulMonoBehaviour<T>>());
        }
    }
}