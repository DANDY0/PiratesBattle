using System;
using UnityEngine;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;

namespace Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry
{
    [Serializable]
    public class GameObjectEntry
    {
        public GameObjectEntryKey Key;
        public GameObject GameObject;
    }
}