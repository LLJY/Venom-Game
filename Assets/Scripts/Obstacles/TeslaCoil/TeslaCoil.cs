using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Obstacles.TeslaCoil
{
    public class TeslaCoil: StatefulMonoBehaviour<TeslaCoil>
    {
        [SerializeField]private VisualEffect lightningVfx;
        [SerializeField] private Light light;

        private Quaternion _lightningDefaultRotation; 
        private Transform _lightningVfxTransform;

        private float _defaultImpactOffset = -6.25f;

        private bool _arcingToObject = false;

        private IDisposable _periodicArcCoroutine;
        private IDisposable _arcToObjectCoroutine;
        public override void Awake()
        {
            base.Awake();
            _lightningVfxTransform = lightningVfx.transform;
            _lightningDefaultRotation = _lightningVfxTransform.rotation;
            _periodicArcCoroutine = PeriodicArc().ToObservable().Subscribe();
        }

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

        IEnumerator ArcToObject(Vector3 pos)
        {
            _arcingToObject = true;
            var offset = Vector3.Distance(transform.position, pos);
            offset *= 1.5f;
            lightningVfx.SetFloat("ImpactOffset", offset);
            _lightningVfxTransform.LookAt(pos);
            _lightningVfxTransform.Rotate( 90, 0, 0 );
            yield return MakeArc();
            _arcingToObject = false;
        }

        IEnumerator MakeArc()
        {
            lightningVfx.Play();
            light.enabled = true;
            yield return new WaitForSeconds(0.55f);
            light.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
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