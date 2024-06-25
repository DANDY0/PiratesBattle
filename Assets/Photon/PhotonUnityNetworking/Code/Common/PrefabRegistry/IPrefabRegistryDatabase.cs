using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry
{
    public interface IPrefabRegistryDatabase
    {
        GameObject GetObjectByKey(string key);
    }
}