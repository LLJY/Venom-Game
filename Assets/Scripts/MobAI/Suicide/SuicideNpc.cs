using System.Collections;
using MobAI.Harm;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Suicide
{
    public class SuicideNpc: StatefulMonoBehaviour<SuicideNpc>
    {
        // inspector variables
        public float baseAttackDamage = 5;
        public float baseSpeed = 2;
        public Animator animator;
        public GameObject player;
        public float senseRadius = 50;
        public float attackRadius = 50;
        public float rockSpeed = 5;
        public Rigidbody rock;

        // cache variables for performance
        [HideInInspector]public Transform _playerTransform;
        
        // runtime assigned varaibles
        [HideInInspector]public NavMeshAgent agent;
        public Transform rockParentTransform;

        
        // NPC states
        private SuicideWanderState _wanderState;
        private SuicideAttackState _attackState;

        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            // we are not using a manager, override the manager settings
            enabled = true;
            animator = GetComponent<Animator>();
            
            _wanderState = new SuicideWanderState(this);
            _attackState = new SuicideAttackState(this);
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = baseSpeed;
            CurrentState = _wanderState;
            
            _playerTransform = player.transform;
            // make sure the harm NPC always walks with a sword equipped
            animator.SetBool("Sword Equipped", true);
        }
        
        public override void Update() {
            CurrentState.Update();
            var walkSpeed = agent.velocity.magnitude;
            animator.SetBool("Walking", walkSpeed > 0);
            animator.SetFloat("Walking Speed", walkSpeed);

            // ANYSTATE transitions

            if (Vector3.Distance(transform.position, _playerTransform.position) < senseRadius)
            {
                if (CurrentState != _attackState)
                {
                    CurrentState = _attackState;
                }
            }
            else if (CurrentState != _wanderState)
            {
                CurrentState = _wanderState;
            }
        }


    }
}