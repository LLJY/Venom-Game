using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    // inspector assigned variables
    [SerializeField] private GameObject spikes;
    [SerializeField] private float spikeOffsetUp;
    [SerializeField] private float spikeOffsetDown;
    [SerializeField] private float plateOffsetUp;
    [SerializeField] private float plateOffsetDown;
    [SerializeField] private float lerpTime=2;
    
    // runtime assigned variables
    private List<int> objectsInTrigger = new List<int>();
    private Coroutine _coroutine;

    private bool isRaised = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objectsInTrigger.Count > 0 && !isRaised)
        {
            isRaised = true;
            Debug.Log("Spikes Up");
            if (_coroutine != null) return;
            _coroutine = MainThreadDispatcher.StartCoroutine(SetSpikePosition(plateOffsetDown, spikeOffsetUp));
        }
        else if (objectsInTrigger.Count == 0 && isRaised)
        {
            isRaised = false;
            Debug.Log("Spikes Down");
            if (_coroutine != null) return;
            _coroutine = MainThreadDispatcher.StartCoroutine(SetSpikePosition(plateOffsetUp, spikeOffsetDown));
        }
    }

    private IEnumerator SetSpikePosition(float platePosition, float spikePosition)
    {
        var plateStartPos = transform.position;
        var plateEndPos = new Vector3(plateStartPos.x, platePosition, plateStartPos.z);
        var spikeStartPos = spikes.transform.localPosition;
        var spikeEndPos = new Vector3(spikeStartPos.x, spikePosition, spikeStartPos.z);
        var lerpSteps = 100f;
        for (int i = 0; i < lerpSteps; i++)
        {
            var lerp = i / lerpSteps;
            transform.position = Vector3.Lerp(plateStartPos, plateEndPos, lerp);
            spikes.transform.localPosition = Vector3.Lerp(spikeStartPos, spikeEndPos, lerp);
            yield return new WaitForSeconds(lerpTime/lerpSteps);
        }
        _coroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsInTrigger.Add(other.gameObject.GetInstanceID());
    }

    private void OnTriggerExit(Collider other)
    {
        objectsInTrigger.Remove(other.gameObject.GetInstanceID());
    }
}
