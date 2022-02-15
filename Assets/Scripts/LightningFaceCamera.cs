using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFaceCamera : MonoBehaviour
{
    [SerializeField]private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        var direction = _camera.transform.position - pos;
        direction = new Vector3(direction.x, pos.y, direction.z);
        
        transform.LookAt(direction);
    }
}
