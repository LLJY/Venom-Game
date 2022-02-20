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

        /// <summary>
        /// Not strictly necessary, but it takes some time for the bulletObjectPool to be
        /// Assigned to the GameCache static class, so wait for a bit before attempting to set the
        /// instance of _pool
        /// </summary>
        /// <returns></returns>
        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(0.1f);
            _pool = GameCache.bulletObjectPool;
        }

        private void OnTriggerEnter(Collider other)
        {
            // damage whatever that the bullet hits
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