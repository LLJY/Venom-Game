using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobAI.NpcCommon
{
    public class NpcWander<T>: State<T>
    where T: BaseNpc<T>
    {
        private IDisposable _wanderCoroutine;
        public float wanderRadius;
        public NpcWander(T behaviour, float radius) : base(behaviour)
        {
            wanderRadius = radius;
        }

        public override void Prepare()
        {
            Debug.Log($"{typeof(T).FullName} Wander State");
        }

        public override void CleanUp()
        {
            _wanderCoroutine?.Dispose();
        }
        
        public override void Update()
        {
            if (_wanderCoroutine != null || _behaviour.agent.velocity.magnitude > 0) return;
            _wanderCoroutine = WanderMove().ToObservable().Subscribe();
        }

        /// <summary>
        /// Wanders the navmesh agent around a radius of 5 (in any direction) from its current position
        /// adds a randomized delay.
        /// </summary>
        private IEnumerator WanderMove()
        {
            var waitSeconds = Random.Range(0, 10) * 0.5f;
            yield return new WaitForSeconds(waitSeconds);
            var wander = new Vector3(Random.Range(-wanderRadius, wanderRadius), 0, Random.Range(-wanderRadius, wanderRadius));
            _behaviour.agent.SetDestination(_behaviour.transform.position + wander);
            _wanderCoroutine = null;
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}