using System;
using System.Collections;
using MobAI.NpcCommon;
using UniRx;
using UnityEngine;

namespace Obstacles.Turret
{
    public class Bullet: StatefulMonoBehaviour<Bullet>
    {
        public int damage = 20;
        [SerializeField] private BulletObjectPool _pool;
        public override void Awake()
        {
            base.Awake();
            LateStart().ToObservable().Subscribe();
        }

        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.1f);
            _pool = GameCache.bulletObjectPool;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 7)
            {
                NpcCommon.DamageAnything(other.gameObject, damage);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _pool.ReturnToPool(gameObject);
        }
        
        
    }
}