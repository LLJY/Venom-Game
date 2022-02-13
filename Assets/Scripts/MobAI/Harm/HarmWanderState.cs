using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MobAI.Harm
{
    public class HarmWanderState: State<HarmNpc>
    {
        private bool _wanderCoroutineRunning = false;

        public HarmWanderState(HarmNpc behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Harm NPC Wander State");
            var wander = new Vector3(Random.Range(1f, 5f), 0, Random.Range(1f, 5f));
            _behaviour.agent.SetDestination(_behaviour.transform.position + wander);
        }

        public override void CleanUp()
        {
        }

        public override void Update()
        {
            if (!_wanderCoroutineRunning && _behaviour.agent.velocity.magnitude == 0)
            {
                Debug.Log("moving...");
                MainThreadDispatcher.StartCoroutine(WanderMove());
            }
        }

        /// <summary>
        /// Wanders the navmesh agent around a radius of 5 (in any direction) from its current position
        /// adds a randomized delay.
        /// </summary>
        private IEnumerator WanderMove()
        {
            _wanderCoroutineRunning = true;
            var waitSeconds = Random.Range(0, 10) * 0.5f;
            Debug.Log("waiting for seconds" + waitSeconds);
            yield return new WaitForSeconds(waitSeconds);
            var wander = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            _behaviour.agent.SetDestination(_behaviour.transform.position + wander);
            _wanderCoroutineRunning = false;
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}