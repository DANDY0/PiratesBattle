using System;
using UnityEngine;
using static Utils.Enumerators;

namespace Models
{
    [Serializable]
    public class GameObjectEntry
    {
        public GameObjectEntryKey Key;
        public GameObject GameObject;
    }
}