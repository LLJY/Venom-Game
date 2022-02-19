using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretShootingState : State<TurretBehaviour>
    {
        private BulletObjectPool _bulletPool;
        private bool _shouldRunCoroutine;
        private IDisposable _colourCoroutine;

        public TurretShootingState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            _shouldRunCoroutine = true;
            _behaviour.animator.SetBool("Shoot", true);
            _colourCoroutine = _behaviour.LerpTurretColour(Color.red).ToObservable().Subscribe();
        }

        public override void CleanUp()
        {
            _shouldRunCoroutine = false;
            _behaviour.animator.SetBool("Shoot", false);
            _colourCoroutine?.Dispose();
        }

        public override void Update()
        {
            if (_bulletPool == null)
            {
                _bulletPool = GameCache.bulletObjectPool;
            }

            // cache frequently accessed fields
            var playerTransform = _behaviour.player.transform;
            var turretTransform = _behaviour.turretTop.transform;

            var playerTurretDistance = Vector3.Distance(playerTransform.position, turretTransform.position);
            // set the turret active
            if (playerTurretDistance < _behaviour.activeRadius / 2)
            {
                _behaviour.MoveTurretToTransform(playerTransform);
                if (_shouldRunCoroutine)
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
            _shouldRunCoroutine = false;
            Transform[] barrelTransforms = {_behaviour.leftBarrel.transform, _behaviour.rightBarrel.transform};
            foreach (var barrel in barrelTransforms)
            {
                _bulletPool.ShootBullet(barrel, _behaviour.bulletSpeed, Quaternion.LookRotation(_behaviour.player.transform.position - barrel.position));
                yield return new WaitForSeconds(0.5f);
            }
            _shouldRunCoroutine = true;
        }
    }
}