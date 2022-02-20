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
        public bool isEntrance = false;
        
        // without this, the trigger may trigger more than once, causing the game to freak out
        private bool _exit = false;

        private void Start()
        {
            SetDoorPosition();
            _exit = false;
            enabled = false;
        }

        private void SetDoorPosition()
        {
            // hardcoded door positions to -10 of the attached wall's z coordinate, which makes it just right at the corner
            var adjacentPos = adjacentWall.transform.position;
            var wallPos = wall.transform.position;
            transform.position = new Vector3(Mathf.Sign(adjacentPos.x) * -10 + adjacentPos.x, 0, Mathf.Sign(wallPos.z) * -1.1f + wallPos.z);
        }

        // Triggers are unaffected by enable, so we can still play the door anim
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                animator.SetBool("open", true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (Vector3.Distance(transform.position, other.transform.position) > 2 || _exit || !other.gameObject.tag.Equals("Player", StringComparison.InvariantCultureIgnoreCase)) return;
            _exit = true;
            if (isEntrance)
            {
                WorldManager.PreviousWorld();
            }
            else
            {
                WorldManager.NextWorld();
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
