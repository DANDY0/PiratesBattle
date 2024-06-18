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
            SpawnPhotonPool(GameObjectEntryKey.Bullet.ToString(), 20);
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

        private void CheckPoolsAreReady()
        {
            var isAllReady = _dictionary.Keys.All(key => _dictionary[key].IsReady);

            if (isAllReady) 
                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPoolsPrepared, true);
        }

        private void SpawnPhotonPool(string key, int amount)
        {
            _dictionary.Add(key, new PreparationPoolVo { Components = new List<Component>(), InitialSize = amount });
            for (var i = 0; i < amount; i++)
                PhotonNetwork.InstantiatePoolObject(key, Vector3.zero, Quaternion.identity, true);
        }
    }

    public class PreparationPoolVo
    {
        public List<Component> Components;
        public int InitialSize;
        public bool IsReady;
    }
}