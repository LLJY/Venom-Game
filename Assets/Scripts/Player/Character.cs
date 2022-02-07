using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// TODO move camera logic into separate cam script
namespace Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private float playerSprintSpeed = 5;
        [SerializeField] private float characterJumpHeight = 2;

        // GetComponent assigned variables
        private CharacterController _characterController;
        private Animator _animator;
        private Vector3 _currentCharacterVelocity;

        // Start is called before the first frame update
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
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
                var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
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
                    _currentCharacterVelocity.y = Mathf.Sqrt(2 * -Physics.gravity.y * characterJumpHeight);
                }
            }
            else
            {
                // add gravity
                _currentCharacterVelocity.y += Physics.gravity.y * Time.deltaTime;
            }

            _characterController.Move(_currentCharacterVelocity * Time.deltaTime);
        }
    }
}
