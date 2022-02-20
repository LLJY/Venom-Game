using System;
using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretActiveState: State<TurretBehaviour>
    {
        private IDisposable _colourCoroutine;
        public TurretActiveState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {

            // change the turret's colour via a micro coroutine
            _colourCoroutine = _behaviour.LerpTurretColour(Color.yellow).ToObservable().Subscribe();
        }

        public override void CleanUp()
        {
            _colourCoroutine?.Dispose();
        }

        public override void Update()
        {
            // cache frequently accessed fields
            var playerTransform = _behaviour.player.transform;
            var turretTransform = _behaviour.turretTop.transform;

            var playerTurretDistance = Vector3.Distance(playerTransform.position, turretTransform.position);
            
            // set the turret active if it crosses the active radius
            if (playerTurretDistance < _behaviour.activeRadius && playerTurretDistance > _behaviour.activeRadius/2)
            {
                _behaviour.MoveTurretToTransform(playerTransform);
            }else if(playerTurretDistance < _behaviour.activeRadius/2)
            {
                // set the turret to shoot if it crosses half of the active radius
                SetState(_behaviour.ShootingState);
            }
            else
            {
                SetState(_behaviour.InactiveState);
            }
        }

        public override void LateUpdate()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}