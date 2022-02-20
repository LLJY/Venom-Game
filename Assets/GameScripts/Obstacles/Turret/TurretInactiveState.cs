using System;
using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretInactiveState: State<TurretBehaviour>
    {
        private IDisposable _colourCoroutine;

        public TurretInactiveState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            _colourCoroutine = _behaviour.LerpTurretColour(Color.green).ToObservable().Subscribe();
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
            // set the turret to inactive if the player's position exceeds the active radius
            if (playerTurretDistance > _behaviour.activeRadius)
            {
                _behaviour.MoveTurretToTransform(_behaviour.turretBase.transform);
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
    }
}