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

        public override void CleanUp()
        {
        }

        public override void Update()
        {
            _behaviour.agent.SetDestination(_behaviour._playerTransform.position);
            if (Vector3.Distance(_behaviour._playerTransform.position, _behaviour.transform.position) < 2)
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
            _behaviour.animator.SetTrigger("Sword Slash");
            // damage player wh
            if (Vector3.Distance(_behaviour._playerTransform.position, _behaviour.transform.position) < 2 &&
                Vector3.Angle(_behaviour._playerTransform.position, _behaviour.transform.position) < 50)
            {
                MainThreadDispatcher.StartCoroutine(GameCache.playerScriptStatic.DamagePlayer(_behaviour.baseAttackDamage));
            }

            yield return new WaitForSeconds(1f);
            _attackCoroutine = null;
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}