using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Patterns
{
    public enum PoolingMethods
    {
        OutOfFrustrum,
        OnCollision
    }

    public abstract class ObjectPool<T> : MonoBehaviour
        where T : StatefulMonoBehaviour<T>
    {
        private T[] _objectScripts;
        private GameObject[] _gameObjects;
        public ReactiveProperty<int> poolSize = new ReactiveProperty<int>(0);
        public GameObject prefab;
        private IDisposable _subscriber;
        public PoolingMethods[] returnToPoolMethod;

        public virtual void Awake()
        {
            UpdatePoolSize(poolSize.Value);
            _subscriber = poolSize.Subscribe();
        }

        private void UpdatePoolSize(int size)
        {
            if (_gameObjects == null || _objectScripts == null || _objectScripts.Length == 0)
            {
                _objectScripts = new T[size];
                _gameObjects = new GameObject[size];
            }
            else
            {
                Array.Resize(ref _objectScripts, size);
                Array.Resize(ref _gameObjects, size);
            }

            for (var i = 0; i < _gameObjects.Length; i++)
            {
                var go = _gameObjects[i];
                if (go != null) continue;
                _gameObjects[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                _objectScripts[i] = _gameObjects[i].GetComponent<T>();
            }
        }

        public virtual void Update()
        {
            // use a for loop as the size of the loop might be updated during this loop
            for (var i = 0; i < _objectScripts.Length; i++)
            {
                var script = _objectScripts[i];
                if (script == null) continue;
                script.Update();
            }
        }

        public virtual void FixedUpdate()
        {
            // use a for loop as the size of the loop might be updated during this loop
            for (var i = 0; i < _objectScripts.Length; i++)
            {
                var script = _objectScripts[i];
                if (script == null) continue;
                script.FixedUpdate();
            }
        }

        public virtual void LateUpdate()
        {
            // use a for loop as the size of the loop might be updated during this loop
            for (var i = 0; i < _objectScripts.Length; i++)
            {
                var script = _objectScripts[i];
                if (script == null) continue;
                script.LateUpdate();
            }
        }

        /// <summary>
        /// Returns this gameobject to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnToPool(GameObject go)
        {
            go.SetActive(false);
        }

        /// <summary>
        /// Takes the first available object from the pool, returns the first object if none is available
        /// </summary>
        /// <returns></returns>
        public GameObject TakeFromPool()
        {
            var go = _gameObjects.FirstOrDefault(x => !x.activeSelf);
            go ??= _gameObjects[0];
            go.SetActive(true);
            return go;
        }

        private void OnDestroy()
        {
            _subscriber.Dispose();
        }
    }
}