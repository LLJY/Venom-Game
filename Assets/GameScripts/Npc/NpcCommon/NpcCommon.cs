using System;
using MobAI.Anxiety;
using MobAI.Harm;
using MobAI.Suicide;
using UniRx;
using UnityEngine;

namespace MobAI.NpcCommon
{
    public static class NpcCommon
    {
        /// <summary>
        /// Only damage NPCs, if the GameObject somehow only contains an npc
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="damage"></param>
        public static void DamageNpc(GameObject npc, int damage)
        {
            if (npc == null) return;
            if (npc.name.StartsWith("Harm", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = npc.GetComponent<HarmNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
            else if (npc.name.StartsWith("Anxiety", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = npc.GetComponent<AnxietyNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
            else if (npc.name.StartsWith("Suicide", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = npc.GetComponent<SuicideNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
        }
        
        /// <summary>
        /// apply damage to both npcs and player
        /// </summary>
        /// <param name="playerOrNpc">gameObject of the object to damage</param>
        /// <param name="damage">damage to inflict on said object</param>
        public static void DamageAnything(GameObject playerOrNpc, int damage)
        {
            if (playerOrNpc == null) return;
            if (playerOrNpc.name.StartsWith("Player", StringComparison.InvariantCultureIgnoreCase))
            {
                MainThreadDispatcher.StartCoroutine(GameCache.playerScript.DamagePlayer(damage));
            }
            else
            {
                DamageNpc(playerOrNpc, damage);
            }
        }
    }
}