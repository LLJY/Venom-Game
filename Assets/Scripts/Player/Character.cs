using System;
using System.Collections;
using System.Collections.Generic;
using Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

// TODO move camera logic into separate cam script
namespace Player
{
    public class Character : MonoBehaviour
    {
        // Inspector assigned variables
        [SerializeField] private float playerSprintSpeed = 5;
        [SerializeField] private float characterJumpHeight = 2;
        [SerializeField] private float smallAttackTimeout = 5;
        [SerializeField] private float mediumAttackTimeout = 10;
        [SerializeField] private float bigAttackTimeout = 15;
        [SerializeField] private GameObject laserBeam;
        [SerializeField] private GameObject swordSlash;
        [SerializeField] private GameObject swordSlash2;
        [SerializeField] private GameObject sword;
        [SerializeField] private int health = 20;
        [SerializeField] private Renderer _characterRenderer;

        // GetComponent assigned variables
        private CharacterController _characterController;
        private Animator _animator;
        private Vector3 _currentCharacterVelocity;
        private Rigidbody _rb;

        // runtime assigned variables
        private float _smallAttackTriggeredTime = 0;
        private float _mediumAttackTriggeredTime = 0;
        private bool _holdingSword = false;
        private float _bigAttackTriggeredTime = 0;
        private bool _pausePlayerInput = false;
        private Color _baseColour;
        private Material _characterMaterial;
        private static readonly int BaseColor = Shader.PropertyToID("BaseColor");

        // Start is called before the first frame update
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _characterMaterial = _characterRenderer.material;
            _baseColour = _characterMaterial.color;
        }

        void Update()
        {
            HandleCharacterMovement();
            HandleCharacterAttacks();
            if (health <= 0)
            {
                MainThreadDispatcher.StartCoroutine(Death());
            }
        }

        void HandleCharacterMovement()
        {
            // ensure to only move the character when grounded
            if (_characterController.isGrounded)
            {
                Vector3 input = Vector3.zero;
                if (!_pausePlayerInput)
                {
                    input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // apply sprint if moving forwards
                    if (input.magnitude > 0)
                    {
                        input *= playerSprintSpeed;
                    }
                }

                _animator.SetBool("Walking", input.magnitude > 0);
                _animator.SetFloat("Walking Speed", input.magnitude);
                transform.rotation = Quaternion.LookRotation(transform.forward + input);

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

        void HandleCharacterAttacks()
        {
            if (Input.GetButton("Fire2"))
            {
                MainThreadDispatcher.StartCoroutine(BigAttack());
            }

            if (Input.GetButton("Fire1") && _holdingSword)
            {
                MainThreadDispatcher.StartCoroutine(MediumAttack());
            }

            if (Input.GetKeyDown(KeyCode.F) && _characterController.velocity == Vector3.zero)
            {
                _holdingSword = !_holdingSword;
                _animator.SetBool("Sword Equipped", _holdingSword);
                sword.SetActive(_holdingSword);
            }
        }

        private IEnumerator BigAttack()
        {
            /*
             * Trigger the animation, enable the laser,
             * wait a few frames, untrigger and remove the laser beam
             */
            _pausePlayerInput = true;
            _currentCharacterVelocity = new Vector3(0, 0, 0);
            _bigAttackTriggeredTime = Time.time;
            _animator.SetTrigger("Laser Attack");
            yield return new WaitForSeconds(0.9f);
            laserBeam.SetActive(true);
            yield return new WaitForSeconds(1f);
            _animator.ResetTrigger("Laser Attack");
            laserBeam.SetActive(false);
            _pausePlayerInput = false;
            yield return new WaitForSeconds(bigAttackTimeout);
        }

        private IEnumerator MediumAttack()
        {
            /*
             * Stop the player, wait for the player to stop, Trigger the animation, set the sword slashing animation 1
             * to active, wait a while then set the sword slashing animation 2
             */
            _pausePlayerInput = true;
            _currentCharacterVelocity = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(0.2f);
            _mediumAttackTriggeredTime = Time.time;
            swordSlash.SetActive(true);
            _animator.SetTrigger("Sword Attack");
            yield return new WaitForSeconds(1f);
            swordSlash.SetActive(false);
            swordSlash2.SetActive(true);
            _animator.ResetTrigger("Sword Attack");
            yield return new WaitForSeconds(1f);
            swordSlash2.SetActive(false);
            _pausePlayerInput = false;
            yield return new WaitForSeconds(mediumAttackTimeout);
        }

        public IEnumerator DamagePlayer(int damage)
        {
            var time = 0.5f;
            health -= damage;
            _characterMaterial.color = Color.red;
            // lerp back to the original colour
            var time1 = time / 10;
            for (int i = 0; i < 10; i++)
            {
                _characterMaterial.color = Color.Lerp(Color.red, _baseColour, (float) i / 10f);
                yield return new WaitForSeconds(time1);
            }
            
        }

        public IEnumerator Death()
        {
            _animator.SetTrigger("Death");
            // TODO trigger user interface for death.
            yield return null;
        }

    }
}