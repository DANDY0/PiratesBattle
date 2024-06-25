using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Photon.PhotonUnityNetworking.Code.Common.Factory
{
    public class GameFactory : IGameFactory, ISceneContainerInjectable
    {
        private DiContainer _di;
        private readonly IPrefabRegistryDatabase _prefabRegistryDatabase;
        private readonly Dictionary<string, GameObject> _resourceCache = new();

        public GameFactory
        (
            IPrefabRegistryDatabase prefabRegistryDatabase
        )
        {
            _prefabRegistryDatabase = prefabRegistryDatabase;
        }

        public void SetSceneContainer(DiContainer container)
        {
            _di = container;
        }

        public GameObject CreateFromResources(string path, Vector3 posToSpawn, Quaternion rotation)
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

            var instance = _di.InstantiatePrefab(prefab, posToSpawn, rotation, null);

            return instance;
        }

        public GameObject CreateWithKey(string key, Vector3 posToSpawn, Quaternion rotation)
        {
            var prefab = _prefabRegistryDatabase.GetObjectByKey(key);
            var instance = _di.InstantiatePrefab(prefab, posToSpawn, rotation, null);
            return instance;
        }


        public T Create<T>(T prefab, Vector3 posToSpawn, Quaternion rotation) where T : Component
        {
            if (prefab == null)
            {
                throw new ArgumentException($"Prefab not found.");
            }

            var instance = _di.InstantiatePrefab(prefab, posToSpawn, rotation, null);

            return instance.GetComponent<T>();
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