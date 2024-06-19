using System.Collections.Generic;
using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.Pool
{
    public class PoolInfoVo
    {
        public readonly Queue<Component> Objects;
        public readonly Transform Parent;

        public PoolInfoVo(Queue<Component> objectPool, Transform parent)
        {
            Objects = objectPool;
            Parent = parent;
        }
    }
}