using System;
using System.Collections;
using MobAI.Anxiety;
using MobAI.Harm;
using MobAI.Suicide;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MobAI.NpcCommon
{
    public abstract class BaseNpc<T>: StatefulMonoBehaviour<T>
    where T: StatefulMonoBehaviour<T>
    {
        public float baseSpeed = 2;
        public float baseHealth = 20;
        public Animator animator;
        public GameObject player;
        public ReactiveProperty<float> currentHealth = new ReactiveProperty<float>();
        public Image healthBar;
        public Transform cameraTransform;
        public Transform canvasTransform;
        public Renderer npcRenderer;

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
            playerTransform = player.transform;
            RandomizeHealth();
            currentHealth.Value = baseHealth;
        }

        public virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameCache.playerStatic;
            animator = GetComponent<Animator>();
            agent.speed = baseSpeed;
            playerTransform = player.transform;

            currentHealth.Subscribe((x) =>
            {
                Debug.Log($"npc health {x}");
                if (x <= 0 && _deathCoroutine == null)
                {
                    _deathCoroutine = Death().ToObservable().Subscribe();
                }
                healthBar.fillAmount = (Mathf.Max(x, 0) / baseHealth);
            });
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
        
        private IEnumerator Death()
        {
            animator.SetTrigger("Death");
            yield return new WaitForSeconds(2f);
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