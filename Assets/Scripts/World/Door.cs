using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace World
{
    /// <summary>
    /// Opens Opportunities among other things
    ///
    /// Simple script to set the door's position to the wall
    /// </summary>
    public class Door : MonoBehaviour
    {
        public GameObject wall;
        public GameObject adjacentWall;
        public Animator animator;

        void Update()
        {
            /*
             * The world script sets the scale in Start() and there is a real possibility that
             * the scale is set BEFORE we get the opportunity to update the door position
             * update it here after all the scripts have done initializing and then disable the script
             * to prevent further updates.
             *
             * Set the door's x position such that is is near the adjacent door, so it forces the player to traverse the entire map
             * from one door to the other instead of just walking straight.
             * Raycast towards the wall that the door is supposed to be at, then set the coordinates to the hit point
             * this ensures that the door is always on the SURFACE of the wall and not inside of it.
             */
            var wallPos = adjacentWall.transform.position;
            var doorPos =
                new Vector3( Mathf.Sign(wallPos.x)*-10+wallPos.x, transform.position.y, transform.position.z);
            var wallLayerMask = 1 << 3;
            RaycastHit hit;
            var ray = new Ray(doorPos, wall.transform.position - transform.position);
            Physics.Raycast(ray, out hit, 1000f, wallLayerMask);
            transform.position = new Vector3(doorPos.x, doorPos.y, hit.point.z) + transform.up * -0.1f;
            enabled = false;
        }

        // Triggers are unaffected by enable, so we can still play the door anim
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                animator.SetBool("open", true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                animator.SetBool("open", false);
            }
        }
    }

}
