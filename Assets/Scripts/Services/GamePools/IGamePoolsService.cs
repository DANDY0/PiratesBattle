using System.Collections.Generic;
using UnityEngine;

namespace Services.GamePools
{
    public interface IGamePoolsService
    {
        public Dictionary<string, int[]> PhotonViewIdsDictionary { get; }
        void PreparePools();
        void SetItemReady(string key, Component component);
    }
}