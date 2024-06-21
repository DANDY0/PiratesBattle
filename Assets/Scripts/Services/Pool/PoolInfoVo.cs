using System.Collections.Generic;
using UnityEngine;

namespace Services.Pool
{
    public class PoolInfoVo
    {
        public readonly Queue<object> Objects;
        public readonly Transform Parent;

        public PoolInfoVo(Transform parent, Queue<object> objects = null)
        {
            Objects = objects ?? new Queue<object>();
            Parent = parent;
        }
    }
}