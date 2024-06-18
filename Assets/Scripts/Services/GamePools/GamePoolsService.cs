using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using ScriptsPhotonCommon.Pool;
using UnityEngine;
using Utils.Extensions;
using static Utils.Enumerators;

namespace Services.GamePools
{
    public class GamePoolsService : IGamePoolsService
    {
        private readonly IPoolService _poolService;
        private readonly Dictionary<string, PreparationPoolVo> _dictionary = new();

        public GamePoolsService
        (
            IPoolService poolService
        )
        {
            _poolService = poolService;
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
            foreach (var comp in poolVo.Components) 
                _poolService.AddObjectToPool(key, comp);
            CheckPoolsAreReady();
        }

        private void InitializePoolSizes()
        {
            InitializePool(GameObjectEntryKey.Bullet, 50);
        }

        private void InitializePool(GameObjectEntryKey gameObjectEntryKey, int size) => _dictionary.Add(gameObjectEntryKey.ToString(), new PreparationPoolVo(size));

        private void SpawnPhotonPools()
        {
            foreach (var key in _dictionary.Keys)
                for (var i = 0; i < _dictionary[key].InitialSize; i++) 
                    PhotonNetwork.InstantiatePoolObject(key, Vector3.zero, Quaternion.identity, true);
        }

        private void CheckPoolsAreReady()
        {
            var isAllReady = _dictionary.Keys.All(key => _dictionary[key].IsReady);

            if (isAllReady) 
                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPoolsPrepared, true);
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