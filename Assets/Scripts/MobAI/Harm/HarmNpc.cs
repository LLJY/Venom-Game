using MobAI.NpcCommon;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Harm
{
    public class HarmNpc: BaseNpc<HarmNpc>
    {
        #region Inspector Assigned Variables
        public int baseAttackDamage = 5;
        public float senseRadius = 5;
        #endregion
        
        #region States
        private NpcWander<HarmNpc> _wanderState;
        private HarmAttackState _attackState;
        #endregion
        
        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            // we are not using a manager, override the manager settings
            enabled = true;
            animator = GetComponent<Animator>();
            
            _wanderState = new NpcWander<HarmNpc>(this, 5f);
            _attackState = new HarmAttackState(this);
        }

        public override void Start()
        {
            base.Start();
            CurrentState = _wanderState;
            
            // make sure the harm NPC always walks with a sword equipped
            animator.SetBool("Sword Equipped", true);
        }
        
        public override void Update() {
            base.Update();
            CurrentState.Update();

            #region AnyState Transitions
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
            #endregion

        }
    }
}