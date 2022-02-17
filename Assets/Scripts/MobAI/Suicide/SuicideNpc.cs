using System.Collections;
using MobAI.Harm;
using MobAI.NpcCommon;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Suicide
{
    public class SuicideNpc: BaseNpc<SuicideNpc>
    {
        // inspector variables
        public float baseAttackDamage = 5;
        public float senseRadius = 50;
        public float attackRadius = 25;
        public float rockSpeed = 5;
        public Rigidbody rock;
        public Transform rockParentTransform;

        
        // NPC states
        private NpcWander<SuicideNpc> _wanderState;
        private SuicideAttackState _attackState;

        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            // we are not using a manager, override the manager settings
            enabled = true;
            animator = GetComponent<Animator>();
            
            _wanderState = new NpcWander<SuicideNpc>(this, 10f);
            _attackState = new SuicideAttackState(this);
        }

        public override void Start()
        {
            base.Start();
            CurrentState = _wanderState;
        }
        
        public override void Update() {
            base.Update();
            CurrentState.Update();

            // ANYSTATE transitions

            if (Vector3.Distance(transform.position, playerTransform.position) < senseRadius)
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