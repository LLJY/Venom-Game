using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameScripts.Npc
{
    public class MobSpawner: MonoBehaviour
    {
        [SerializeField] private GameObject _anxietyMobPrefab;
        [SerializeField] private GameObject _harmMobPrefab;
        [SerializeField] private GameObject _suicideMobPrefab;
        public float baseSpawnRate = 0.2f;
        public int baseMaxSpawns = 10;

        /// <summary>
        /// mob spawn rate in seconds
        /// </summary>
        private float _mobSpawnRate;

        /// <summary>
        /// maximum number of mobs to spawn
        /// </summary>
        private float _maxMobs;

        private IDisposable _spawnerCoroutine;

        private void Awake()
        {
            // apply rng to the spawn rate and maximum mobs
            _mobSpawnRate = baseSpawnRate * Random.Range(0f, 2f);
            _maxMobs = baseMaxSpawns * Random.Range(0f, 2f);
            _spawnerCoroutine = Spawner().ToObservable().Subscribe();
        }

        private int ChooseMobSpawn()
        {
            // choose mobs as int indexes 0 = anxiety, 1 = harm, 2 = suicide
            // 5% chance of suicide, 80% chance of harm, 20% chance of anxiety
            var mobChance = Random.Range(0f, 1f);
            return mobChance < 0.05f ? 2 : mobChance < 0.85f ? 1 : 0;
        }

        private IEnumerator Spawner()
        {
            for (int i = 0; i< _maxMobs; i++)
            {
                var worldSize = GameCache.worldSize;
                var randomPos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), 0,
                    Random.Range(-worldSize.z / 2, worldSize.z / 2));
                switch (ChooseMobSpawn())
                {
                    case 0:
                    {
                        Instantiate(_anxietyMobPrefab, randomPos, Quaternion.identity);
                        break;
                    }
                    case 1:
                    {
                        Instantiate(_harmMobPrefab, randomPos, Quaternion.identity);
                        break;
                    }
                    case 2:
                    {
                        Instantiate(_suicideMobPrefab, randomPos, Quaternion.identity);
                        break;
                    }
                }

                yield return new WaitForSeconds(1 / _mobSpawnRate);
            }
        }

        private void OnDestroy()
        {
            _spawnerCoroutine.Dispose();
        }
    }
}