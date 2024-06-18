using System.Collections.Generic;
using UnityEngine;

namespace ScriptsPhotonCommon.Pool
{
    public class PoolInfoVo
    {
        public Queue<Component> Objects;
        public Transform Parent;

        public PoolInfoVo(Queue<Component> objectPool, Transform parent)
        {
            Objects = objectPool;
            Parent = parent;
        }
    }
}