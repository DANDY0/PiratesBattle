using System;
using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry
{
    [Serializable]
    public class GameObjectEntry
    {
        public Enumerators.GameObjectEntryKey Key;
        public GameObject GameObject;
    }
}