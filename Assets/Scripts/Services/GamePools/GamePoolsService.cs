using System;
using System.Collections.Generic;
using System.Linq;
using Photon.PhotonUnityNetworking.Code.Common.Pool;
using Photon.Pun;
using PunNetwork.Services.CustomProperties;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static Utils.Enumerators;

namespace Services.GamePools
{
    public class GamePoolsService : IGamePoolsService, IInitializable, IDisposable
    {
        private readonly IPoolService _poolService;
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly Dictionary<string, PreparationPoolVo> _dictionary = new();
        public Dictionary<string, int[]> PhotonViewIdsDictionary { get; private set; }

        public GamePoolsService
        (
            IPoolService poolService,
            ICustomPropertiesService customPropertiesService
        )
        {
            _poolService = poolService;
            _customPropertiesService = customPropertiesService;
        }

        public void Initialize()
        {
            _customPropertiesService.RoomPoolsViewIdsChangedEvent += OnRoomPoolsViewIdsChangedEvent;
        }

        public void Dispose()
        {
            _customPropertiesService.RoomPoolsViewIdsChangedEvent -= OnRoomPoolsViewIdsChangedEvent;
        }

        public void PreparePools()
        {
            InitializePoolSizes();
            if (PhotonNetwork.IsMasterClient)
                SpawnPhotonPools();
        }

        public void SetItemReady(string key, Component component)
        {
            var poolVo = _dictionary[key];
            poolVo.Components.Add(component);
            if (poolVo.Components.Count != poolVo.InitialSize) return;
            poolVo.IsReady = true;

            CheckPoolsAreReady();
        }

        private void InitializePoolSizes()
        {
            InitializePool(GameObjectEntryKey.Bullet, 20);
        }

        private void InitializePool(GameObjectEntryKey gameObjectEntryKey, int size) =>
            _dictionary.Add(gameObjectEntryKey.ToString(), new PreparationPoolVo(size));

        private void SpawnPhotonPools()
        {
            foreach (var key in _dictionary.Keys)
                for (var i = 0; i < _dictionary[key].InitialSize; i++)
                    PhotonNetwork.InstantiatePoolObject(key, Vector3.zero, Quaternion.identity, true);
        }

        private void CheckPoolsAreReady()
        {
            var isAllReady = _dictionary.Keys.All(key => _dictionary[key].IsReady);

            if (!isAllReady) return;

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonViewIdsDictionary = _dictionary.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Components.Select(component => component.GetComponent<PhotonView>().ViewID)
                        .ToArray()
                );
            }

            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPoolsPrepared, true);

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var key in _dictionary.Keys)
                    foreach (var component in _dictionary[key].Components) 
                        _poolService.AddObjectToPool(key, component);

                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPoolsPreparedAndSynced, true);
            }
        }

        private void OnRoomPoolsViewIdsChangedEvent(Dictionary<string, int[]> updatedPoolsViewIds)
        {
            if (PhotonNetwork.IsMasterClient)
                return;
            foreach (var (key, viewIDs) in updatedPoolsViewIds)
            {
                if (_dictionary.TryGetValue(key, out var poolVo))
                {
                    SortComponentsByViewID(poolVo.Components, viewIDs);
                    foreach (var comp in poolVo.Components)
                        _poolService.AddObjectToPool(key, comp);
                    
                    PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPoolsPreparedAndSynced, true);
                }
                else
                    Debug.LogWarning($"Key {key} not found in _dictionary.");
            }
        }

        private static void SortComponentsByViewID(ICollection<Component> components, IEnumerable<int> sortedViewIDs)
        {
            var componentDict = components.ToDictionary(c => c.GetComponent<PhotonView>().ViewID);

            components.Clear();
            foreach (var viewID in sortedViewIDs)
            {
                if (componentDict.TryGetValue(viewID, out var component))
                    components.Add(component);
                else
                    Debug.LogWarning($"ViewID {viewID} not found in components.");
            }
        }
    }

    public class PreparationPoolVo
    {
        public readonly List<Component> Components;
        public readonly int InitialSize;
        public bool IsReady;

        public PreparationPoolVo(int initialSize)
        {
            Components = new List<Component>();
            InitialSize = initialSize;
        }
    }
}