using System;
using UnityEngine;

namespace Player
{
    public class Camera: MonoBehaviour
    {
        // inspector assigned variables
        [SerializeField] private GameObject player;
        [SerializeField]private float cameraDampTime = 1;
        [SerializeField]private float cameraOffsetFromPlayer;
        [SerializeField]private Vector3 cameraAngles;

        // runtime assigned variables
        public Transform cameraTarget;
        private Vector3 _cameraOffset;
        private Vector3 _cameraCurrentVelocity;
        private RaycastHit oldHit;
        private Color oldHitColor;
        
        // cached variables
        private int _wallLayerMask = 1 << 3;

        private void Start()
        {
            cameraTarget = player.transform;
            // let the player script set the camera angles from the player.
            var playerPos = cameraTarget.position;
        
            transform.position = playerPos + new Vector3(0, 0, cameraOffsetFromPlayer);
            transform.RotateAround(playerPos, new Vector3(0, 1, 0), cameraAngles.x);
            transform.RotateAround(playerPos, new Vector3(1, 0, 0), cameraAngles.y);
            transform.LookAt(playerPos);

            _cameraOffset = transform.position - playerPos;
        }
        
        private void LateUpdate()
        {
            // camera follow
            var cameraPos = Vector3.SmoothDamp(transform.position, _cameraOffset + cameraTarget.position, ref _cameraCurrentVelocity,
                cameraDampTime);
            transform.position = cameraPos;
        }
        
        private void FixedUpdate()
        {
            var playerPos = cameraTarget.position;
            var ray = new Ray(playerPos,  transform.position - playerPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, _wallLayerMask))
            {
                if (oldHit.transform != null && hit.transform.gameObject.GetInstanceID() ==
                    oldHit.transform.gameObject.GetInstanceID()) return;
                Debug.Log("hit!");
                var hitMaterial = hit.collider.gameObject.GetComponent<MeshRenderer>().material;
                oldHit = hit;
                oldHitColor = hitMaterial.color;
                hitMaterial.color = new Color(oldHitColor.r, oldHitColor.g, oldHitColor.b, 0.2f);
            }
            else
            {
                if (oldHit.transform != null)
                {
                    var hitMaterial = oldHit.transform.gameObject.GetComponent<MeshRenderer>().material;
                    hitMaterial.color = oldHitColor;
                    // unassign it to default
                    oldHit = new RaycastHit();
                }
            }
        }

        /// <summary>
        /// Changes the camera to the specified transform
        /// </summary>
        /// <param name="targetTransform">the target to focus on</param>
        public void ChangeCameraTarget(Transform targetTransform)
        {
            cameraTarget = targetTransform;
        }

        /// <summary>
        /// Resets the camera to point to the player
        /// </summary>
        public void ResetCameraTarget()
        {
            cameraTarget = player.transform;
        }
    }
}