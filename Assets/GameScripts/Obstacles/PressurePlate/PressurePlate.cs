using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MobAI.NpcCommon;
using UniRx;
using UnityEngine;

namespace Obstacles.PressurePlate
{
    public class PressurePlate : StatefulMonoBehaviour<PressurePlate>
    {
        // inspector assigned variables
        [SerializeField] private GameObject spikes;
        [SerializeField] private float spikeOffsetUp;
        [SerializeField] private float spikeOffsetDown;
        [SerializeField] private float plateOffsetUp;
        [SerializeField] private float plateOffsetDown;
        [SerializeField] private float lerpTime=2;
        [SerializeField] private int damagePerSecond = 5;

        // runtime assigned variables
        private List<GameObject> objectsInTrigger = new List<GameObject>();
        private IDisposable _spikePositionCoroutine;
        private IDisposable _damagePlayerCoroutine;
        private Vector3 _transformPos;
        private int _damageableLayerMask = 1 << 7;

        private bool isRaised = false;
        // Start is called before the first frame update
        public override void Awake()
        {
            base.Awake();
            _transformPos = transform.position;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (objectsInTrigger.Count > 0 && !isRaised)
            {
                isRaised = true;
                if (_spikePositionCoroutine != null) return;
                _spikePositionCoroutine = SetSpikePosition(plateOffsetDown, spikeOffsetUp).ToObservable().Subscribe();
                _damagePlayerCoroutine = DamageObjects().ToObservable().Subscribe();
            }
            else if (objectsInTrigger.Count == 0 && isRaised)
            {
                isRaised = false;
                if (_spikePositionCoroutine != null) return;
                _spikePositionCoroutine = SetSpikePosition(plateOffsetUp, spikeOffsetDown).ToObservable().Subscribe();
            }
        }

        private IEnumerator DamageObjects()
        {
            Collider[] hitColliders = new Collider[4];
            Physics.OverlapSphereNonAlloc(_transformPos, 1f, hitColliders, _damageableLayerMask);
            while (objectsInTrigger.Count > 0)
            {
                objectsInTrigger.RemoveAll(x => Vector3.Distance(x.transform.position, transform.position) > 2f);
                foreach (var obj in hitColliders)
                {
                    if (obj == null) continue;
                    NpcCommon.DamageAnything(obj.gameObject, damagePerSecond);
                }
                yield return new WaitForSeconds(1f);
            }
            _damagePlayerCoroutine?.Dispose();
            _damagePlayerCoroutine = null;
        }

        private IEnumerator SetSpikePosition(float platePosition, float spikePosition)
        {
            var plateStartPos = transform.position;
            var plateEndPos = new Vector3(plateStartPos.x, platePosition, plateStartPos.z);
            var spikeStartPos = spikes.transform.localPosition;
            var spikeEndPos = new Vector3(spikeStartPos.x, spikePosition, spikeStartPos.z);
            const float lerpSteps = 100f;
            for (int i = 0; i < lerpSteps; i++)
            {
                var lerp = i / lerpSteps;
                transform.position = Vector3.Lerp(plateStartPos, plateEndPos, lerp);
                spikes.transform.localPosition = Vector3.Lerp(spikeStartPos, spikeEndPos, lerp);
                yield return new WaitForSeconds(lerpTime/lerpSteps);
            }
            _spikePositionCoroutine?.Dispose();
            _spikePositionCoroutine = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            objectsInTrigger.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            objectsInTrigger.RemoveAll(x=>x.GetInstanceID() == other.gameObject.GetInstanceID());
        }

        public override void OnDestroy()
        {
            _damagePlayerCoroutine?.Dispose();
            _spikePositionCoroutine?.Dispose();
        }
    }
}
