using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Harm
{
    public class HarmNpc: StatefulMonoBehaviour<HarmNpc>
    {
        // inspector variables
        public int baseAttackDamage = 5;
        public float baseSpeed = 2;
        public Animator animator;
        public GameObject player;
        public float senseRadius = 5;

        // cache variables for performance
        [HideInInspector]public Transform _playerTransform;
        
        // runtime assigned varaibles
        [HideInInspector]public NavMeshAgent agent;
        
        // NPC states
        private HarmWanderState _wanderState;
        private HarmAttackState _attackState;

        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            // we are not using a manager, override the manager settings
            enabled = true;
            animator = GetComponent<Animator>();
            
            _wanderState = new HarmWanderState(this);
            _attackState = new HarmAttackState(this);
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