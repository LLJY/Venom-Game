using System;
using System.Collections;
using System.Collections.Generic;
using MobAI.NpcCommon;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Obstacles.TeslaCoil
{
    public class TeslaCoil : StatefulMonoBehaviour<TeslaCoil>
    {
        // inspector assigned variables
        [SerializeField] private VisualEffect lightningVfx;
        [SerializeField] private Light light;
        public int damage = 20;
        
        // runtime assigned variables
        private Quaternion _lightningDefaultRotation;
        private Transform _lightningVfxTransform;
        private float _defaultImpactOffset = -6.25f;
        private bool _arcingToObject = false;
        private IDisposable _periodicArcCoroutine;
        private IDisposable _arcToObjectCoroutine;
        private Vector3 _transformPos;
        private int _damageableLayerMask = 1 << 7;

        public override void Awake()
        {
            base.Awake();
            _lightningVfxTransform = lightningVfx.transform;
            _lightningDefaultRotation = _lightningVfxTransform.rotation;
            _periodicArcCoroutine = PeriodicArc().ToObservable().Subscribe();

            _transformPos = transform.position;
        }

        /// <summary>
        /// Coroutine that periodically shoots a lightning bolt upwards as long as the tesla coil
        /// is not currently arcing to something else already.
        /// </summary>
        /// <returns></returns>
        IEnumerator PeriodicArc()
        {
            while (isActiveAndEnabled)
            {
                if (!_arcingToObject)
                {
                    lightningVfx.SetFloat("ImpactOffset", _defaultImpactOffset);
                    _lightningVfxTransform.rotation = _lightningDefaultRotation;
                    yield return MakeArc();
                }

                var randomArcTime = Random.Range(3, 10);
                yield return new WaitForSeconds(randomArcTime);
            }
        }

        /// <summary>
        /// shoots a lightning bolt at a Vector 3 position and try to damage it
        /// using the NpcCommon.DamageAnything utility script
        /// </summary>
        /// <param name="pos">the target position</param>
        /// <returns></returns>
        IEnumerator ArcToObject(Vector3 pos)
        {
            _arcingToObject = true;
            var distanceToObject = Vector3.Distance(_transformPos, pos);
            var offset = distanceToObject * 1.5f;
            lightningVfx.SetFloat("ImpactOffset", offset);
            _lightningVfxTransform.LookAt(pos);
            _lightningVfxTransform.Rotate(90, 0, 0);
            yield return MakeArc();

            RaycastHit[] hitColliders = new RaycastHit[2];
            Physics.SphereCastNonAlloc(new Ray(_transformPos, pos - _transformPos), 0.5f, hitColliders,
                distanceToObject, _damageableLayerMask);
            foreach (var obj in hitColliders)
            {
                if (obj.collider == null) continue;
                NpcCommon.DamageAnything(obj.collider.gameObject, damage);
            }

            _arcingToObject = false;
        }

        /// <summary>
        /// Make the actual lightning using 
        /// </summary>
        /// <returns></returns>
        IEnumerator MakeArc()
        {
            lightningVfx.Play();
            light.enabled = true;
            yield return new WaitForSeconds(0.55f);
            light.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            // arc to an object if it enters the trigger
            if (_arcingToObject) return;
            _arcToObjectCoroutine = ArcToObject(other.transform.position).ToObservable().Subscribe();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _periodicArcCoroutine?.Dispose();
            _arcToObjectCoroutine?.Dispose();
        }
    }
}