using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using Player;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Suicide
{
    public class Rock: MonoBehaviour
    {
        // inspector assigned variables
        public int baseDamage=5;
        public int explosionRadius = 5;
        public float explosionForce = 10;
        public Animator suicideAnimator;

        // runtime assigned variables
        private Collider[] _overlapResults = new Collider[]{};
        private int _damageableLayerMask = 1 << 7;

        private Rigidbody _rb;
        private Transform _parentTransform;
        private Vector3 _originalScale;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            enabled = false;
            _parentTransform = transform.parent;
            _originalScale = transform.localScale;
        }

        private void OnCollisionEnter(Collision collision)
        {
            /*
             * ignore if the collision is with the npc
             * do an overlapsphere to detect objects around to damage and to add force to
             */
            if (collision.gameObject.name.StartsWith("Suicide", StringComparison.InvariantCultureIgnoreCase)) return;
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders, _damageableLayerMask);
            foreach (var overlap in hitColliders)
            {
                if (overlap == null) continue;
                var rb = overlap.GetComponent<Rigidbody>();
                if (rb == null) return;
                if (overlap.tag.Equals("Player", StringComparison.InvariantCultureIgnoreCase))
                {
                    var cc = overlap.GetComponent<CharacterController>();
                    MainThreadDispatcher.StartCoroutine(Knockback(overlap.transform.position, rb, cc));
                }
                else
                {
                    var navmesh = overlap.GetComponent<NavMeshAgent>();
                    MainThreadDispatcher.StartCoroutine(Knockback(overlap.transform.position, rb, navmesh));
                }
            }
            MainThreadDispatcher.StartCoroutine(ResetRock());

        }
        
        /// <summary>
        /// applies knockback to the player character
        /// </summary>
        /// <param name="point">explosion point</param>
        /// <param name="rb">player rigidbody</param>
        /// <param name="controller">player character controller</param>
        /// <returns></returns>
        public IEnumerator Knockback(Vector3 point, Rigidbody rb, CharacterController controller)
        {
            if (!rb.isKinematic) yield break;
            rb.isKinematic = false;
            controller.enabled = false;
            rb.AddExplosionForce(explosionForce, point, explosionRadius);
            MainThreadDispatcher.StartCoroutine(GameCache.playerScript.DamagePlayer(baseDamage));
            while (rb.velocity != Vector3.zero)
            {
                yield return new WaitForSeconds(1);
            }
            controller.enabled = true;
            rb.isKinematic = true;
        }
        
        /// <summary>
        /// applies knockback to the NPCs only
        /// </summary>
        /// <param name="point">point at which to apply the explosive force from</param>
        /// <param name="rb">Rigidbody belonging to the NPC</param>
        /// <param name="controller">NavMeshAgent belonging to the NPC</param>
        /// <returns></returns>
        public IEnumerator Knockback(Vector3 point, Rigidbody rb, NavMeshAgent controller)
        {
            if (!rb.isKinematic) yield break;
            rb.isKinematic = false;
            controller.enabled = false;
            rb.AddExplosionForce(explosionForce, point, explosionRadius);
            while (rb.velocity != Vector3.zero)
            {
                yield return new WaitForSeconds(1);
            }
            controller.enabled = true;
            rb.isKinematic = true;
        }
        
        /// <summary>
        /// Coroutine that triggers the SuicideNpc's animation and then resets the rock's scale and position.
        /// </summary>
        /// <returns></returns>
        public IEnumerator ResetRock()
        {
            /*
             * Set the animation, wait for it to complete, then re-parent the object and reset the scale
             * and position.
             */
            if (!_rb.isKinematic)
            {
                var renderer = _rb.GetComponent<MeshRenderer>();
                renderer.enabled = false;
                suicideAnimator.SetTrigger("Pick Up");
                yield return new WaitForSeconds(1.8f);
                transform.SetParent(_parentTransform, false);
                transform.localScale = _originalScale;
                _rb.isKinematic = true;
                _rb.transform.localPosition = Vector3.zero;
                renderer.enabled = true;
                suicideAnimator.ResetTrigger("Pick Up");
            }
        }

    }
}