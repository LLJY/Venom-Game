using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace MobAI.Suicide
{
    public class SuicideAttackState: State<SuicideNpc>
    {
        private IDisposable _attackCoroutine = null;
        public SuicideAttackState(SuicideNpc behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Harm NPC attack state");
        }

        public override void CleanUp()
        {
            _attackCoroutine?.Dispose();
        }

        public override void Update()
        {
            _behaviour.agent.SetDestination(_behaviour.playerTransform.position);
            if (Vector3.Distance(_behaviour.playerTransform.position, _behaviour.transform.position) < _behaviour.attackRadius / 2)
            {
                _behaviour.agent.isStopped = true;
                if (_attackCoroutine == null)
                {
                    _attackCoroutine = Attack().ToObservable().Subscribe();
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
                yield return new WaitForSeconds(1.6f);
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