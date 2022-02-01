using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// TODO move camera logic into separate cam script
public class Character : MonoBehaviour
{
    [FormerlySerializedAs("camera")] [SerializeField]private Camera mainCamera;
    [SerializeField]private Vector3 cameraAngles;
    [SerializeField]private float cameraOffsetFromPlayer;
    [SerializeField]private float playerSprintSpeed = 5;
    [SerializeField]private float playerAcceleration = 2;
    [SerializeField]private float cameraDampTime = 1;
    [SerializeField]private float characterJumpHeight = 2;
    
    // GetComponent assigned variables
    private CharacterController _characterController;
    private Animator _animator;
    private Vector3 _cameraCurrentVelocity;
    private Vector3 _currentCharacterVelocity;
    
    // runtime assigned variables
    private RaycastHit oldHit;
    private Color oldHitColor;
    private Vector3 _cameraOffset;

    // cache some variables for performance reasons
    private Transform _cameraTransform;
    private int _wallLayerMask = 1 << 3;
    // Start is called before the first frame update
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        // let the player script set the camera angles from the player.
        var transformPos = transform.position;
        _cameraTransform = mainCamera.transform;
        _cameraTransform.position = transformPos + new Vector3(0, 0, cameraOffsetFromPlayer);
        _cameraTransform.RotateAround(transformPos, new Vector3(0, 1, 0), cameraAngles.x);
        _cameraTransform.RotateAround(transformPos, new Vector3(1, 0, 0), cameraAngles.y);
        _cameraTransform.LookAt(transformPos);

        _cameraOffset = _cameraTransform.position - transformPos;
    }
    
    void Update()
    {
        HandleCharacterMovement();
    }

    void HandleCharacterMovement()
    {
        // ensure to only move the character when grounded
        if (_characterController.isGrounded)
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), 0,Input.GetAxis("Vertical")).normalized;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // apply sprint if moving forwards
                if (input.magnitude > 0)
                {
                    input *= playerSprintSpeed;
                }
            }
            // cache the input.magnitude field
            var inputMag = input.magnitude;
            transform.rotation = Quaternion.LookRotation(transform.forward + input);
            _animator.SetBool("Walking", inputMag > 0);
            _animator.SetFloat("Walking Speed", inputMag);
            
            // implement jump
            input.y = _currentCharacterVelocity.y;
            _currentCharacterVelocity = input;
            if (Input.GetButtonDown("Jump"))
            {
                _currentCharacterVelocity.y = Mathf.Sqrt(2* -Physics.gravity.y * characterJumpHeight);
            }
        }
        else
        {
            // add gravity
            _currentCharacterVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        _characterController.Move(_currentCharacterVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        // camera follow
        var cameraPos = Vector3.SmoothDamp(_cameraTransform.position, _cameraOffset + transform.position, ref _cameraCurrentVelocity,
            cameraDampTime);
        _cameraTransform.position = cameraPos;
    }

    private void FixedUpdate()
    {
        var ray = new Ray(transform.position,  _cameraTransform.position - transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, _wallLayerMask))
        {
            if (oldHit.transform == null || hit.transform.gameObject.GetInstanceID() != oldHit.transform.gameObject.GetInstanceID())
                {
                    Debug.Log("hit!");
                    var hitMaterial = hit.collider.gameObject.GetComponent<MeshRenderer>().material;
                    oldHit = hit;
                    oldHitColor = hitMaterial.color;
                    hitMaterial.color = new Color(oldHitColor.r, oldHitColor.g, oldHitColor.b, 0.2f);
                }
        }
        else
        {
            if (oldHit.transform != null)
            {
                var hitMaterial = oldHit.transform.gameObject.GetComponent<MeshRenderer>().material;
                hitMaterial.color = oldHitColor;
                // unassign it to default
                oldHit = new RaycastHit();
            }
        }
    }
}
