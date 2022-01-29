using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Turret
{
    public class TurretShootingState : State<TurretBehaviour>
    {
        private bool shouldRunCoroutine;

        public TurretShootingState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Shooting State");
            shouldRunCoroutine = true;
            _behaviour.animator.SetBool("Shoot", true);
            MainThreadDispatcher.StartCoroutine(_behaviour.LerpTurretColour(Color.red));
        }

        public override void CleanUp()
        {
            shouldRunCoroutine = false;
            _behaviour.animator.SetBool("Shoot", false);
        }

        public override void Update()
        {
            // cache frequently accessed fields
            var playerTransform = _behaviour.player.transform;
            var turretTransform = _behaviour.turretTop.transform;

            var playerTurretDistance = Vector3.Distance(playerTransform.position, turretTransform.position);
            // set the turret active
            if (playerTurretDistance < _behaviour.activeRadius / 2)
            {
                _behaviour.MoveTurretToTransform(playerTransform);
                if (shouldRunCoroutine)
                {
                    MainThreadDispatcher.StartCoroutine(Shoot());
                }
            }
            else
            {
                SetState(_behaviour.ActiveState);
            }
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }

        IEnumerator Shoot()
        {
            shouldRunCoroutine = false;
            Transform[] barrelTransforms = {_behaviour.leftBarrel.transform, _behaviour.rightBarrel.transform};
            foreach (var barrel in barrelTransforms)
            {
                // left first then right
                var bullet = GameObject.Instantiate(_behaviour.bulletPrefab, barrel.position + (barrel.forward),
                    Quaternion.LookRotation(_behaviour.player.transform.position - barrel.position));
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * _behaviour.bulletSpeed,
                    ForceMode.VelocityChange);
                yield return new WaitForSeconds(0.5f);
            }

            shouldRunCoroutine = true;
        }
    }
}