using UnityEngine;

namespace GameScripts.Npc
{
    public class HealthDrop: Drop
    {
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            // add between 10 and 20 hp
            GameCache.playerScript.health.Value += (float)Random.Range(10, 20);
        }
    }
}