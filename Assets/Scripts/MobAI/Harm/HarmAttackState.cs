using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;

namespace MobAI.Harm
{
    public class HarmAttackState: State<HarmNpc>
    {
        private Coroutine _attackCoroutine = null;
        public HarmAttackState(HarmNpc behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Harm NPC attack state");
        }

        public override void Update()
        {
            /*
             * Go to the player, if within distance of 2, attack the player
             */
            _behaviour.agent.SetDestination(_behaviour.playerTransform.position);
            if (Vector3.Distance(_behaviour.playerTransform.position, _behaviour.transform.position) < 2)
            {
                _behaviour.agent.isStopped = true;
                if (_attackCoroutine == null)
                {
                    _attackCoroutine = MainThreadDispatcher.StartCoroutine(Attack());
                }
            }
            else
            {
                _behaviour.agent.isStopped = false;
            }
        }

        private IEnumerator Attack()
        {
            /*
             * Ensure that the attack is valid, then damage the player.
             */
            _behaviour.animator.SetTrigger("Sword Slash");
            if (Vector3.Distance(_behaviour.playerTransform.position, _behaviour.transform.position) < 2 &&
                Vector3.Angle(_behaviour.playerTransform.position, _behaviour.transform.position) < 50)
            {
                MainThreadDispatcher.StartCoroutine(GameCache.playerScriptStatic.DamagePlayer(_behaviour.baseAttackDamage));
            }

            yield return new WaitForSeconds(1f);
            _attackCoroutine = null;
        }

        #region Unused Event Functions

        public override void CleanUp()
        {
        }
        
        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }

        #endregion


    }
}