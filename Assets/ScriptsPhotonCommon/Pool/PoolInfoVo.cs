using System.Collections.Generic;
using UnityEngine;

namespace ScriptsPhotonCommon.Pool
{
    public class PoolInfoVo
    {
        public Queue<object> Objects;
        public Transform Parent;

        public PoolInfoVo(Queue<object> objectPool, Transform parent)
        {
            Objects = objectPool;
            Parent = parent;
        }
    }
}