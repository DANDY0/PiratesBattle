using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using IPrefabProvider = ScriptsPhotonCommon.PrefabProvider.IPrefabProvider;
using Object = UnityEngine.Object;

namespace ScriptsPhotonCommon.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly DiContainer _di;
        private readonly IPrefabProvider _prefabProvider;
        private readonly Dictionary<string, GameObject> _resourceCache = new();

        public GameFactory
        (
            DiContainer di,
            IPrefabProvider prefabProvider
        )
        {
            _di = di;
            _prefabProvider = prefabProvider;
        }

        public GameObject CreateWithPath(string path, Vector3 posToSpawn, Quaternion rotation)
        {
            var cached = _resourceCache.TryGetValue(path, out var prefab);
            if (!cached)
            {
                prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                    Debug.LogError($"{nameof(GameFactory)} Prefab with path {path} not found.");
                else
                    _resourceCache.Add(path, prefab);
            }

            var instance = Object.Instantiate(prefab, posToSpawn, rotation);
            _di.InjectGameObject(instance);
            return instance;
        }
        
        public GameObject CreateWithKey(string key, Vector3 posToSpawn, Quaternion rotation)
        {
            var prefab = _prefabProvider.GetPrefab(key);
            var instance = Object.Instantiate(prefab, posToSpawn, rotation);
            _di.InjectGameObject(instance);
            return instance;
        }

        
        public T Create<T>(T prefab, Vector3 posToSpawn, Quaternion rotation) where T : Component
        {
            if (prefab == null)
            {
                throw new ArgumentException($"Prefab not found.");
            }

            var instance = Object.Instantiate(prefab, posToSpawn, rotation);
            return instance;
        }

        
        public GameObject Create(GameObject prefab, Vector3 posToSpawn, Quaternion rotation)
        {
            if (prefab == null)
            {
                throw new ArgumentException($"Prefab not found.");
            }

            var instance = Object.Instantiate(prefab, posToSpawn, rotation);
            return instance;
        }
        
        public GameObject Create(GameObject prefab, Vector3 posToSpawn, Quaternion rotation, Transform parent)
        {
            if (prefab == null)
            {
                throw new ArgumentException($"Prefab not found.");
            }

            var instance = Object.Instantiate(prefab, posToSpawn, rotation, parent);
            return instance;
        }

    }
}