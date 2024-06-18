using ScriptsPhotonCommon.Factory;
using ScriptsPhotonCommon.Pool;
using UnityEngine;

namespace ScriptsPhotonCommon.PhotonFactory
{
    public class PhotonFactory : IPhotonFactory
    {
        private readonly IGameFactory _gameFactory;
        private readonly IPoolService _poolService;

        public PhotonFactory
        (
            IGameFactory gameFactory,
            IPoolService poolService
        )
        {
            _gameFactory = gameFactory;
            _poolService = poolService;
        }
        
        public T Instantiate<T>(string key, Vector3 position, Quaternion rotation) where T : class
        {
            if (_poolService.ContainsPool(key))
                return _poolService.ActivatePoolItem<T>(key, position, rotation);

            return _gameFactory.CreateWithKey(key, position, rotation) as T;
        }

        public void Destroy(GameObject gameObject, string key = null)
        {
            if (key == null)
            {
                Object.Destroy(gameObject);
                return;
            }
            
            _poolService.DisablePoolItem(key, gameObject);
        }
    }
}