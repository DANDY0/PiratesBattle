using System;

namespace ScriptsPhotonCommon.Pool
{
    [Serializable]
    public class PoolObjectDataVo
    {
        public string Key; // Pool Key
        public bool Ifs; // Is First Spawn
    }
}