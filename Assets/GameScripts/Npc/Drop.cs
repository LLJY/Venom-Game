using System;
using UnityEngine;

namespace GameScripts.Npc
{
    public abstract class Drop: MonoBehaviour
    {
        public virtual void Update()
        {
            // rotates 100 degrees per second
            transform.Rotate (0,100*Time.deltaTime,0);
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.name.StartsWith("Player", StringComparison.InvariantCultureIgnoreCase)) return;
            Destroy(gameObject);
        }
    }
}