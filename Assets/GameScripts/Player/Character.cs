using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using MobAI.Anxiety;
using MobAI.Harm;
using MobAI.NpcCommon;
using MobAI.Suicide;
using Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
    public class Character : MonoBehaviour
    {
        // Inspector assigned variables
        [SerializeField] private float playerSprintSpeed = 5;
        [SerializeField] private float characterJumpHeight = 2;
        [SerializeField] private int mediumAttackTimeout = 10;
        [SerializeField] private int bigAttackTimeout = 15;
        [SerializeField] private GameObject laserBeam;
        [SerializeField] private GameObject swordSlash;
        [SerializeField] private GameObject swordSlash2;
        [SerializeField] private GameObject sword;
        [SerializeField] private int baseHealth;
        [SerializeField] private Renderer characterRenderer;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image xpBar;
        [SerializeField] private Text xpBarText;
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private Image mediumAttackImage;
        [SerializeField] private Text mediumAttackCountdownText;
        [SerializeField] private Image bigAttackImage;
        [SerializeField] private Text bigAttackCountdownText;
        [SerializeField] private Canvas dieScreen;


        // GetComponent assigned variables
        private CharacterController _characterController;
        private Animator _animator;
        private Vector3 _currentCharacterVelocity;
        private Rigidbody _rb;

        // runtime assigned variables
        private bool _holdingSword = false;
        private bool _pausePlayerInput = false;
        private Color _baseColour;
        private Material _characterMaterial;
        private static readonly int BaseColor = Shader.PropertyToID("BaseColor");
        private IDisposable _dieCoroutine;
        [HideInInspector] public ReactiveProperty<float> health = new ReactiveProperty<float>(20);
        private int _npcLayerMask = 1 << 7;
        private Vector3 _playerCenter;
        private IDisposable _mediumAttackCoroutine;
        private IDisposable _bigAttackCoroutine;


        // Start is called before the first frame update
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _playerCenter = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            _characterMaterial = characterRenderer.material;
            _baseColour = _characterMaterial.color;
        }

        private void Start()
        {
            Debug.Log("playerlevel" + GameCache.GameData.PlayerLevelRaw);
            baseHealth *= Mathf.FloorToInt(1 + GameCache.GameData.PlayerLevel / 25);
            baseDamage *= Mathf.FloorToInt(1 + GameCache.GameData.PlayerLevel / 30);
            
            Debug.Log("player health" + GameCache.GameData.PlayerCurrentHealth);

            // give the player full health on respawn
            if (GameCache.GameData.FirstTimePlaying || GameCache.GameData.FreshRespawn)
            {
                GameCache.GameData.FreshRespawn = false;
                health.Value = baseHealth;
                GameCache.GameData.PlayerCurrentHealth = baseHealth;
            }
            else
            {
                health.Value = GameCache.GameData.PlayerCurrentHealth;
            }
            
            health.Subscribe((x) =>
            {
                if (health.Value <= 0 && _dieCoroutine == null)
                {
                    _dieCoroutine = Death().ToObservable().Subscribe();
                }

                GameCache.GameData.PlayerCurrentHealth = x;
                healthBar.fillAmount = (float) x / (float) baseHealth;
            });
            
            // update the xp bar
            GameCache.GameData.PlayerXpReactiveProperty.Subscribe(x =>
            {
                xpBar.fillAmount = 1 - GameCache.GameData.PlayerLevelProgress;
                xpBarText.text = GameCache.GameData.PlayerLevel.ToString();
            });
        }

        void Update()
        {
            HandleCharacterMovement();
            HandleCharacterAttacks();
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
            if (Input.GetButton("Fire2") && _bigAttackCoroutine == null)
            {
                _bigAttackCoroutine = BigAttack().ToObservable().Subscribe();
            }

            if (Input.GetButton("Fire1") && _holdingSword && _mediumAttackCoroutine == null)
            {
                _mediumAttackCoroutine = MediumAttack().ToObservable().Subscribe();
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
            _animator.SetTrigger("Laser Attack");
            yield return new WaitForSeconds(0.9f);
            laserBeam.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            var hitColliders = new RaycastHit[10];
            Physics.RaycastNonAlloc(new Ray(transform.position, transform.forward), hitColliders, 10f, _npcLayerMask);
            foreach (var ray in hitColliders)
            {
                if (ray.collider == null) continue;
                NpcCommon.DamageNpc(ray.collider.gameObject, baseDamage * 3);
            }

            _animator.ResetTrigger("Laser Attack");
            laserBeam.SetActive(false);
            _pausePlayerInput = false;
            MainThreadDispatcher.StartCoroutine(SetAttackUserInterfaceTimeout(bigAttackImage, bigAttackCountdownText,
                bigAttackTimeout));
            yield return new WaitForSeconds(bigAttackTimeout);

            _bigAttackCoroutine = null;
        }

        private IEnumerator MediumAttack()
        {
            /*
             * Stop the player, wait for the player to stop, Trigger the animation, set the sword slashing animation 1
             * to active, wait a while then set the sword slashing animation 2
             */
            _pausePlayerInput = true;
            _currentCharacterVelocity = new Vector3(0, 0, 0);
            var damage = baseDamage * 1.5f;
            // first slash
            yield return new WaitForSeconds(0.2f);
            swordSlash.SetActive(true);
            _animator.SetTrigger("Sword Attack");

            yield return new WaitForSeconds(0.5f);
            Collider[] hitColliders = new Collider[10];
            Physics.OverlapSphereNonAlloc(transform.position, 3f, hitColliders, _npcLayerMask);
            foreach (var npc in hitColliders)
            {
                if (npc == null) continue;
                NpcCommon.DamageNpc(npc.gameObject, Mathf.FloorToInt(damage));
            }

            yield return new WaitForSeconds(0.5f);

            // second slash
            swordSlash.SetActive(false);
            swordSlash2.SetActive(true);
            _animator.ResetTrigger("Sword Attack");
            yield return new WaitForSeconds(1f);
            Physics.OverlapSphereNonAlloc(_playerCenter, 3f, hitColliders, _npcLayerMask);
            foreach (var npc in hitColliders)
            {
                if (npc == null) continue;
                NpcCommon.DamageNpc(npc.gameObject, Mathf.FloorToInt(damage));
            }

            swordSlash2.SetActive(false);
            _pausePlayerInput = false;
            MainThreadDispatcher.StartCoroutine(SetAttackUserInterfaceTimeout(mediumAttackImage,
                mediumAttackCountdownText, mediumAttackTimeout));
            yield return new WaitForSeconds(mediumAttackTimeout);

            _mediumAttackCoroutine = null;
        }

        public IEnumerator DamagePlayer(int damage)
        {
            var time = 0.5f;
            health.Value -= damage;
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
            Debug.Log("Death");
            _pausePlayerInput = true;
            _animator.SetTrigger("Death");
            yield return new WaitForSeconds(4f);
            dieScreen.enabled = true;
            yield return null;
        }

        private IEnumerator SetAttackUserInterfaceTimeout(Image image, Text countdownText, int countdownTime)
        {
            var startColor = image.color;
            image.color = new Color(startColor.r, startColor.g, startColor.b, 0.2f);
            countdownText.enabled = true;
            for (int i = 0; i < countdownTime; i++)
            {
                countdownText.text = (countdownTime - i).ToString();
                yield return new WaitForSeconds(1f);
            }

            image.color = startColor;
            countdownText.enabled = false;
        }

        private void OnDestroy()
        {
            _dieCoroutine?.Dispose();
        }
    }
}