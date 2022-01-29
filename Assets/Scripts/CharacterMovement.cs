using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]private Camera camera;
    [SerializeField]private Vector3 cameraAngles;
    [SerializeField]private float cameraOffsetFromPlayer;
    [SerializeField]private float playerSprintSpeed = 5;
    [SerializeField] private float playerAcceleration = 2;
    [SerializeField] private float cameraDampTime = 1;
    [SerializeField] private float characterJumpHeight = 2;
    
    private Vector3 _cameraOffset;

    private CharacterController _characterController;
    private Animator _animator;

    private Vector3 _cameraCurrentVelocity;

    private Vector3 _currentCharacterVelocity;
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
        var cameraTransform = camera.transform;
        cameraTransform.position = transformPos + new Vector3(0, 0, cameraOffsetFromPlayer);
        cameraTransform.RotateAround(transformPos, new Vector3(0, 1, 0), cameraAngles.x);
        cameraTransform.RotateAround(transformPos, new Vector3(1, 0, 0), cameraAngles.y);
        cameraTransform.LookAt(transformPos);

        _cameraOffset = cameraTransform.position - transformPos;
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
        var cameraTransform = camera.transform;
        // camera follow
        var cameraPos = Vector3.SmoothDamp(cameraTransform.position, _cameraOffset + transform.position, ref _cameraCurrentVelocity,
            cameraDampTime);
        cameraTransform.position = cameraPos;

    }
}
