using System.Collections.Generic;
using UnityEngine;

namespace ScriptsPhotonCommon.PrefabProvider
{
    public class PrefabProvider : IPrefabProvider
    {
        private readonly Dictionary<string, GameObject> _prefabDictionary;

        public PrefabProvider
        (
            Dictionary<string, GameObject> prefabDictionary
        )
        {
            _prefabDictionary = prefabDictionary;
        }

        public GameObject GetPrefab(string name)
        {
            if (_prefabDictionary.TryGetValue(name, out var prefab))
                return prefab;

            throw new KeyNotFoundException($"Prefab with name {name} not found.");
        }
    }
}