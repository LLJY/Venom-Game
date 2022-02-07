using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MobAI
{
    /// <summary>
    /// Anxiety NPC will try to stay away from the player, it does not have any attack capability
    /// However, it will give all nearby mobs a buff
    /// </summary>
    public class AnxietyNpc: StatefulMonoBehaviour<AnxietyNpc>
    {
        // inspector variables
        public float baseBuffRadius = 5;
        public float baseSpeed = 2;
        public Animator animator;
        public GameObject player;
        public float avoidRadius = 5;

        // cache variables for performance
        [HideInInspector]public Transform _playerTransform;
        
        // runtime assigned varaibles
        [HideInInspector]public NavMeshAgent agent;
        
        // NPC states
        private AnxietyWanderState _wanderState;
        private AnxietyAvoidState _avoidState;

        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            // we are not using a manager, override the manager settings
            enabled = true;
            animator = GetComponent<Animator>();
            
            _wanderState = new AnxietyWanderState(this);
            _avoidState = new AnxietyAvoidState(this);
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = baseSpeed;
            CurrentState = _wanderState;

            _playerTransform = player.transform;
        }

        public override void Update() {
            CurrentState.Update();
            var walkSpeed = agent.velocity.magnitude;
            var horizontalSpeed = agent.velocity.x;
            animator.SetBool("Walking", walkSpeed > 0);
            animator.SetFloat("Walking Speed", walkSpeed);

            // ANYSTATE transitions

            if (Vector3.Distance(transform.position, _playerTransform.position) < avoidRadius)
            {
                if (CurrentState != _avoidState)
                {
                    CurrentState = _avoidState;
                }
            }
            else if (CurrentState != _wanderState)
            {
                CurrentState = _wanderState;
            }
        }
        
    }
}

