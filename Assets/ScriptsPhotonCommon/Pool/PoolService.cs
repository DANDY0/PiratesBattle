using System;
using System.Collections.Generic;
using ScriptsPhotonCommon.Factory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptsPhotonCommon.Pool
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<string, PoolInfoVo> _poolDictionary = new();
        private readonly IGameFactory _gameFactory;

        public PoolService
        (
            IGameFactory gameFactory
        )
        {
            _gameFactory = gameFactory;
        }

        public void SpawnPool<T>(string key, int amount) where T : Component
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                var objectPool = new Queue<Component>();
                var parent = new GameObject($"[Pool] {key}").transform;

                for (var i = 0; i < amount; i++)
                {
                    var component = _gameFactory.CreateWithKey(key, Vector3.zero, Quaternion.identity)
                        .GetComponent<T>();
                    component.transform.SetParent(parent);
                    component.gameObject.SetActive(false);
                    objectPool.Enqueue(component);
                }

                _poolDictionary.Add(key, new PoolInfoVo(objectPool, parent));
            }
            else
            {
                Debug.LogWarning("Pool with key " + key + " already exists.");
            }
        }

        public void AddObjectToPool(string key, Component obj)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                var parent = new GameObject($"[Pool] {key}").transform;

                _poolDictionary.Add(key, new PoolInfoVo(new Queue<Component>(), parent));
            }

            var poolInfoVo = _poolDictionary[key];

            poolInfoVo.Objects.Enqueue(obj);

            if (obj is null) return;
            obj.transform.SetParent(poolInfoVo.Parent);
            obj.gameObject.SetActive(false);
        }

        public T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class
        {
            if (!_poolDictionary.TryGetValue(key, out var poolInfoVo))
            {
                Debug.LogWarning($"Pool with key {key} doesn't exist.");
                return null;
            }

            var objectToSpawn = poolInfoVo.Objects.Count == 0
                ? CreateAndReturnNewInstance<Component>(key)
                : poolInfoVo.Objects.Dequeue();

            var go = objectToSpawn.gameObject;
            go.SetActive(true);
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.SetParent(poolInfoVo.Parent);

            if (typeof(T) == typeof(GameObject))
                return go as T;

            if (objectToSpawn is T castedObject)
                return castedObject;

            return null;
        }

        public void DisablePoolItem<T>(string key, T obj) where T : class
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning("Pool with key " + key + " doesn't exist.");
                return;
            }

            if (obj == null)
            {
                Debug.LogWarning("The object to disable is null.");
                return;
            }

            GameObject go = null;
            Component component = null;

            switch (obj)
            {
                case GameObject gameObject:
                    go = gameObject;
                    component = go.GetComponent<Component>();
                    break;
                case Component comp:
                    go = comp.gameObject;
                    component = comp;
                    break;
            }

            if (go != null)
                go.SetActive(false);
            else
            {
                Debug.LogWarning("The object is neither a GameObject nor a Component.");
                return;
            }

            if (component != null)
                _poolDictionary[key].Objects.Enqueue(component);
            else
                Debug.LogWarning("No component found to add to the pool.");
        }

        public void RemovePool(string key)
        {
            if (!_poolDictionary.TryGetValue(key, out var pool)) return;
            pool.Objects.Clear();
            _poolDictionary.Remove(key);
        }

        public bool ContainsPool(string key) => _poolDictionary.ContainsKey(key);

        private T CreateAndReturnNewInstance<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(key));
            }

            Component newObj = _gameFactory.CreateWithKey(key, Vector3.zero, Quaternion.identity).transform;
            newObj.gameObject.SetActive(false);

            return newObj as T;
        }
    }
}