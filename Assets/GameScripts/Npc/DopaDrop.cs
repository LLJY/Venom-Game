using UnityEngine;

namespace GameScripts.Npc
{
    public class DopaDrop: Drop
    {
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            // add 0.1 xp
            GameCache.GameData.PlayerXpReactiveProperty.Value += 0.1f;
        }
    }
}