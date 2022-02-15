using UnityEngine;
using UnityEngine.AI;

namespace MobAI.NpcCommon
{
    public abstract class BaseNpc<T>: StatefulMonoBehaviour<T>
    where T: StatefulMonoBehaviour<T>
    {
        public float baseSpeed = 2;
        public float baseHealth = 20;
        public Animator animator;
        public GameObject player;

        // cached variables for performance
        public Transform playerTransform;
        
        // runtime assigned variables
        [HideInInspector]public NavMeshAgent agent;
        public override void Awake()
        {
            base.Awake();
            enabled = true;
            playerTransform = player.transform;
        }

        public virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameCache.playerStatic;
            animator = GetComponent<Animator>();
            agent.speed = baseSpeed;
            playerTransform = player.transform;
        }

        public override void Update()
        {
            base.Update();
            var walkSpeed = agent.velocity.magnitude;
            animator.SetBool("Walking", walkSpeed > 0);
            animator.SetFloat("Walking Speed", walkSpeed);

            if (baseHealth <= 0)
            {
                // TODO play die animation
            }
        }
    }
}