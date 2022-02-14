using System.Collections;
using UniRx;
using UnityEngine;

namespace MobAI.Suicide
{
    public class SuicideAttackState: State<SuicideNpc>
    {
        private Coroutine _attackCoroutine = null;
        public SuicideAttackState(SuicideNpc behaviour) : base(behaviour)
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
            if (Vector3.Distance(_behaviour._playerTransform.position, _behaviour.transform.position) < _behaviour.attackRadius / 2)
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
            if (_behaviour.rock.isKinematic)
            {
                Debug.Log("attack triggered");
                _behaviour.animator.SetTrigger("Throw");
                yield return new WaitForSeconds(2f);
                _behaviour.rock.isKinematic = false;
                _behaviour.rock.transform.SetParent(null, true);
                var directionToPlayer = GameCache.playerStatic.transform.position - _behaviour.transform.position;
                _behaviour.rock.AddForce(directionToPlayer.normalized*_behaviour.rockSpeed, ForceMode.VelocityChange);
                yield return new WaitForSeconds(2f);
            }
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