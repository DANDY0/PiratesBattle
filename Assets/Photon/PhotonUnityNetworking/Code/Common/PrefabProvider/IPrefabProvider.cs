using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.PrefabProvider
{
    public interface IPrefabProvider
    {
        GameObject GetPrefab(string name);
    }
}