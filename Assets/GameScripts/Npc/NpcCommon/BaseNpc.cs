using System;
using System.Collections;
using Game;
using MobAI.Anxiety;
using MobAI.Harm;
using MobAI.Suicide;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Camera = Player.Camera;
using Random = UnityEngine.Random;

namespace MobAI.NpcCommon
{
    public abstract class BaseNpc<T>: StatefulMonoBehaviour<T>
    where T: StatefulMonoBehaviour<T>
    {
        // inspector assigned variables
        public float baseSpeed = 2;
        public float baseHealth = 20;
        public Animator animator;
        [HideInInspector]public GameObject player;
        public ReactiveProperty<float> currentHealth = new ReactiveProperty<float>();
        public Image healthBar;
        public Transform cameraTransform;
        public Transform canvasTransform;
        public Renderer npcRenderer;
        public GameObject dopaDrop;
        public GameObject healthDrop;

        // cached variables for performance
        public Transform playerTransform;
        private Color _npcBaseColor;
        private Material _npcMaterial;
        
        // runtime assigned variables
        [HideInInspector]public NavMeshAgent agent;
        private IDisposable _deathCoroutine;
        public override void Awake()
        {
            base.Awake();
            _npcMaterial = npcRenderer.material;
            _npcBaseColor = _npcMaterial.color;
            enabled = true;
            currentHealth.Value = baseHealth;
        }

        public virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameCache.playerStatic;
            animator = GetComponent<Animator>();
            agent.speed = baseSpeed;
            playerTransform = player.transform;
            RandomizeHealth();
            baseHealth *= Mathf.FloorToInt(1+ GameCache.GameData.PlayerLevel/30);

            currentHealth.Subscribe((x) =>
            {
                Debug.Log($"npc health {x}");
                if (x <= 0 && _deathCoroutine == null)
                {
                    enableStatefulMb = false;
                    agent.velocity = Vector3.zero;
                    agent.speed = 0;
                    _deathCoroutine = Death().ToObservable().Subscribe();
                }
                healthBar.fillAmount = (Mathf.Max(x, 0) / baseHealth);
            });
            cameraTransform = GameCache.CameraScript.transform;

            if (!agent.isOnNavMesh)
            {
                Destroy(gameObject);
            }

        }

        private void RandomizeHealth()
        {
            baseHealth *= Random.Range(0.5f, 1.5f);
        }

        public override void Update()
        {
            base.Update();
            var walkSpeed = agent.velocity.magnitude;
            animator.SetBool("Walking", walkSpeed > 0);
            animator.SetFloat("Walking Speed", walkSpeed);
        }

        public IEnumerator Death()
        {
            animator.SetTrigger("Death");
            yield return new WaitForSeconds(4f);
            GameCache.GameData.DemonsKilled++;
                                
            // drop xp and health (50% chance of dropping health
            Instantiate(dopaDrop, transform.position, Quaternion.identity);
            if (Random.Range(0f, 1f) < 0.5f)
            {
                Instantiate(healthDrop, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        public override void LateUpdate()
        {
            canvasTransform.LookAt(cameraTransform.forward + canvasTransform.position);
        }

        public IEnumerator DamageNpc(int damage)
        {
            var time = 0.5f;
            currentHealth.Value -= damage;
            _npcMaterial.color = Color.red;
            // lerp back to the original colour
            var time1 = time / 10;
            for (int i = 0; i < 10; i++)
            {
                _npcMaterial.color = Color.Lerp(Color.red, _npcBaseColor, (float) i / 10f);
                yield return new WaitForSeconds(time1);
            }
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _deathCoroutine?.Dispose();
        }
    }
}