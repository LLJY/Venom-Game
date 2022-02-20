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
            Debug.Log("Suicide NPC attack state");
        }

        public override void CleanUp()
        {
            _attackCoroutine?.Dispose();
        }

        public override void Update()
        {
            /*
             * move to the player, but stop a distance away, then trigger the attack coroutine
             * note that the rock's position is reset in Rock.cs, along with the pick up animation
             */
            _behaviour.agent.SetDestination(_behaviour.playerTransform.position);
            if (Vector3.Distance(_behaviour.playerTransform.position, _behaviour.transform.position) < _behaviour.attackRadius)
            {
                // Debug.Log("should attack");
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

        /// <summary>
        /// Suicide NPC attack coroutine
        /// </summary>
        /// <returns></returns>
        private IEnumerator Attack()
        {
            /*
             * trigger the throw animation, activate the rigidbody, unparent the rock for independent motion
             * and YEET the rock towards the player's transform/position
             */
            if (_behaviour.rock.isKinematic)
            {
                Debug.Log("attack triggered");
                _behaviour.animator.SetTrigger("Throw");
                yield return new WaitForSeconds(1.6f);
                _behaviour.rock.isKinematic = false;
                _behaviour.rock.transform.SetParent(null, true);
                var directionToPlayer = GameCache.playerStatic.transform.position + new Vector3(0, 0.5f, 0) - _behaviour.transform.position;
                _behaviour.rock.AddForce(directionToPlayer.normalized*_behaviour.rockSpeed, ForceMode.VelocityChange);
            }
            yield return new WaitForSeconds(2f);
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