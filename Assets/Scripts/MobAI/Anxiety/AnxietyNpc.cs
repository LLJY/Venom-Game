using MobAI.NpcCommon;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Anxiety
{
    /// <summary>
    /// Anxiety NPC will try to stay away from the player, it does not have any attack capability
    /// However, it will give all nearby mobs a buff
    /// </summary>
    public class AnxietyNpc: BaseNpc<AnxietyNpc>
    {
        #region Inspector Assigned Variables
        public float baseBuffRadius = 5;
        public float avoidRadius = 5;
        #endregion

        #region States
        private AnxietyAvoidState _avoidState;
        private NpcWander<AnxietyNpc> _wanderState;
        #endregion
        
        // TODO randomize all values
        public override void Awake()
        {
            base.Awake();
            _wanderState = new NpcWander<AnxietyNpc>(this, 5);
            _avoidState = new AnxietyAvoidState(this);
        }

        public override void Start()
        {
            base.Start();
            CurrentState = _wanderState;
        }

        public override void Update() {
            base.Update();
            CurrentState?.Update();

            #region AnyState Transitions

            if (Vector3.Distance(transform.position, playerTransform.position) < avoidRadius)
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

            #endregion


        }
        
    }
}

