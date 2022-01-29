using System;
using System.Collections;
using UnityEngine;

namespace Turret
{
    public class TurretBehaviour: StatefulMonoBehaviour<TurretBehaviour>
    {
        public GameObject turretTop;
        public GameObject turretBase;
        public GameObject leftBarrel;
        public GameObject rightBarrel;
        [HideInInspector]public GameObject player;
        [HideInInspector]public Animator animator;
        public GameObject turretTemperature;
        public Material turretTemperatureMaterialRenderer;
        public GameObject bulletPrefab;
        public float colorChangeTime=1;

        public float rotationSpeed = 2;
        public float bulletSpeed = 10;
        public float activeRadius = 5f;
        
        // states
        public TurretActiveState ActiveState;
        public TurretDestroyedState DestroyedState;
        public TurretShootingState ShootingState;
        public TurretInactiveState InactiveState;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            ActiveState = new TurretActiveState(this);
            DestroyedState = new TurretDestroyedState(this);

            ShootingState = new TurretShootingState(this);

            InactiveState = new TurretInactiveState(this);
            CurrentState = InactiveState;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start()
        {
            turretTemperatureMaterialRenderer = turretTemperature.GetComponent<Renderer>().material;
        }

        /// <summary>
        /// Slowly lerps the turret's location towards the desired rotation
        /// </summary>
        public void MoveTurretToTransform(Transform targetTransform)
        {
            var turretTransform = turretTop.transform;
            Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - turretTransform.position);
            turretTop.transform.rotation =
                Quaternion.Slerp(turretTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public IEnumerator LerpTurretColour(Color color)
        {
            var startColor = turretTemperatureMaterialRenderer.color;
            var startTime = Time.time;
            var currentTime = Time.time;
            var elapsedTime = currentTime - startTime;
            while (elapsedTime < colorChangeTime)
            {
                turretTemperatureMaterialRenderer.color = Color.Lerp(startColor, color, elapsedTime / colorChangeTime);
                elapsedTime = currentTime - startTime;
                yield return new WaitForSeconds(0.05f);
                currentTime = Time.time;
            }
        }
    }
}