using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace MobAI.Suicide
{
    public class Rock: MonoBehaviour
    {
        public int baseDamage=5;
        public int explosionRadius = 5;
        public float explosionForce = 10;
        public Animator suicideAnimator;

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
            Debug.Log(collision.gameObject.name);
            MainThreadDispatcher.StartCoroutine(ResetRock());
            Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, _overlapResults, _damageableLayerMask);
            foreach (var overlap in _overlapResults)
            {
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
            
        }
        
        public IEnumerator Knockback(Vector3 point, Rigidbody rb, CharacterController controller)
        {
            if (rb.isKinematic) yield break;
            controller.enabled = false;
            rb.AddExplosionForce(explosionForce, point, explosionRadius);
            while (rb.velocity != Vector3.zero)
            {
                yield return new WaitForSeconds(1);
            }
            controller.enabled = true;
            rb.isKinematic = true;
        }
        
        public IEnumerator Knockback(Vector3 point, Rigidbody rb, NavMeshAgent controller)
        {
            if (rb.isKinematic) yield break;
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
                suicideAnimator.SetTrigger("Pick Up");
                yield return new WaitForSeconds(2f);
                transform.SetParent(_parentTransform, false);
                transform.localScale = _originalScale;
                _rb.isKinematic = true;
                _rb.transform.localPosition = Vector3.zero;
            }
        }

    }
}