using UnityEngine;

namespace ScriptsPhotonCommon.PhotonFactory
{
    public interface IPhotonFactory
    {
        T Instantiate<T>(string key, Vector3 position, Quaternion rotation) where T : Object;
        void Destroy(GameObject gameObject, string key = null); 
    }
}