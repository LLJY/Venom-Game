using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class TurretInactiveState: State<TurretBehaviour>
    {
        public TurretInactiveState(TurretBehaviour behaviour) : base(behaviour)
        {
        }

        public override void Prepare()
        {
            Debug.Log("Turret Inactive State");
            MainThreadDispatcher.StartCoroutine(_behaviour.LerpTurretColour(Color.green));
        }

        public override void CleanUp()
        {
        }

        public override void Update()
        {
            // cache frequently accessed fields
            var playerTransform = _behaviour.player.transform;
            var turretTransform = _behaviour.turretTop.transform;

            var playerTurretDistance = Vector3.Distance(playerTransform.position, turretTransform.position);
            // set the turret active
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