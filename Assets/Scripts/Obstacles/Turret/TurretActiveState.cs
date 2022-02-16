using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretActiveState: State<TurretBehaviour>
    {
        public TurretActiveState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Active State");

            // change the turret's colour via a micro coroutine
           MainThreadDispatcher.StartCoroutine(_behaviour.LerpTurretColour(Color.yellow));
        }

        public override void CleanUp()
        {
            // TODO think of a cleanup
        }

        public override void Update()
        {
            // cache frequently accessed fields
            var playerTransform = _behaviour.player.transform;
            var turretTransform = _behaviour.turretTop.transform;

            var playerTurretDistance = Vector3.Distance(playerTransform.position, turretTransform.position);
            // set the turret active
            if (playerTurretDistance < _behaviour.activeRadius && playerTurretDistance > _behaviour.activeRadius/2)
            {
                _behaviour.MoveTurretToTransform(playerTransform);
            }else if(playerTurretDistance < _behaviour.activeRadius/2)
            {
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