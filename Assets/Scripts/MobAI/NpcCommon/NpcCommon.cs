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
        
        public static void DamageAnything(GameObject playerOrNpc, int damage)
        {
            if (playerOrNpc == null) return;
            if (playerOrNpc.name.StartsWith("Player", StringComparison.InvariantCultureIgnoreCase))
            {
                MainThreadDispatcher.StartCoroutine(GameCache.playerScript.DamagePlayer(damage));
            }
            else if (playerOrNpc.name.StartsWith("Harm", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = playerOrNpc.GetComponent<HarmNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
            else if (playerOrNpc.name.StartsWith("Anxiety", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = playerOrNpc.GetComponent<AnxietyNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
            else if (playerOrNpc.name.StartsWith("Suicide", StringComparison.InvariantCultureIgnoreCase))
            {
                var script = playerOrNpc.GetComponent<SuicideNpc>();
                MainThreadDispatcher.StartCoroutine(script.DamageNpc(damage));
            }
        }
    }
}