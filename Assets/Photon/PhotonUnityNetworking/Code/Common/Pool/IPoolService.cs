using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.Pool
{
    public interface IPoolService
    {
        void SpawnPool<T>(string key, int amount) where T : Component;
        void AddObjectToPool(string key, Component obj);
        T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class;
        void DisablePoolItem<T>(string key, T obj) where T : class;
        void RemovePool(string key);
        bool ContainsPool(string key);
    }
}