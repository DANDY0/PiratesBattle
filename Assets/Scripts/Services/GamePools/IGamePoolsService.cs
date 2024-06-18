using UnityEngine;

namespace Services.GamePools
{
    public interface IGamePoolsService
    {
        void PreparePools();
        void SetItemReady(string key, Component component);
    }
}