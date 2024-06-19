using System;

namespace Photon.PhotonUnityNetworking.Code.Common.Pool
{
    [Serializable]
    public class PoolObjectDataVo
    {
        public string Key; // Pool Key
        public bool Ifs; // Is First Spawn
    }
}