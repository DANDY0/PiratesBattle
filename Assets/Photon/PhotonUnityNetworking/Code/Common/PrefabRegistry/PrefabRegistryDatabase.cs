using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry
{
    [CreateAssetMenu(menuName = "Databases/PrefabRegistryDatabase", fileName = "PrefabRegistryDatabase")]
    public class PrefabRegistryDatabase : ScriptableObject, IPrefabRegistryDatabase
    {
        [SerializeField] private GameObjectEntry[] _gameObjectEntries;

        private Dictionary<string, GameObject> _entriesDictionary;

        private void OnEnable()
        {
            _entriesDictionary = new Dictionary<string, GameObject>();

            foreach (var entry in _gameObjectEntries) 
                _entriesDictionary.Add(entry.Key.ToString(), entry.GameObject);
        }
        
        public GameObject GetObjectByKey(string key)
        {
            try
            {
                return _entriesDictionary[key];
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"[{nameof(PrefabRegistryDatabase)}] GameObject with name {key} was not present in the dictionary. {e.StackTrace}");
            }
        }
    }
}