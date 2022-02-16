using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;

namespace MobAI.Harm
{
    public class HarmAttackState: State<HarmNpc>
    {
        private IDisposable _attackCoroutine = null;
        private IDisposable _damagePlayerCoroutine = null;
        public HarmAttackState(HarmNpc behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Harm NPC attack state");
        }
        public override void CleanUp()
        {
            _damagePlayerCoroutine?.Dispose();
            _attackCoroutine?.Dispose();
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
                _attackCoroutine ??= Attack().ToObservable().Subscribe();
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
                _damagePlayerCoroutine = GameCache.playerScript.DamagePlayer(_behaviour.baseAttackDamage).ToObservable()
                    .Subscribe();
            }

            yield return new WaitForSeconds(1f);
            _attackCoroutine = null;
        }

        #region Unused Event Functions
        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }
        #endregion


    }
}