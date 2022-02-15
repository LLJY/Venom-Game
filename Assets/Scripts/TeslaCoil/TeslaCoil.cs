using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

namespace TeslaCoil
{
    public class TeslaCoil: MonoBehaviour
    {
        [SerializeField]private VisualEffect lightningVfx;
        [SerializeField] private Light light;

        private Quaternion _lightningDefaultRotation; 
        private Transform _lightningVfxTransform;

        private float _defaultImpactOffset = -6.25f;

        private bool _arcingToObject = false;
        private void Start()
        {
            _lightningVfxTransform = lightningVfx.transform;
            _lightningDefaultRotation = _lightningVfxTransform.rotation;
            MainThreadDispatcher.StartCoroutine(PeriodicArc());
        }

        IEnumerator PeriodicArc()
        {
            while (enabled)
            {
                if (!_arcingToObject)
                {
                    Debug.Log("arcing...");
                    lightningVfx.SetFloat("ImpactOffset", _defaultImpactOffset);
                    _lightningVfxTransform.rotation = _lightningDefaultRotation;
                    yield return MakeArc();
                }
                yield return new WaitForSeconds(2f);
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
            Debug.Log($"arcing to object... {other.name}");
            MainThreadDispatcher.StartCoroutine(ArcToObject(other.transform.position));
        }
    }
}