using System;
using PunNetwork.NetworkData;

namespace PunNetwork.Services.CustomProperties
{
    public interface ICustomPropertiesService 
    {
        public event Action PlayerSpawnedEvent;
        event Action PlayerLivesChangedEvent;
        public event Action<NetworkDataModel.PlayerSpawnedData> GetPlayerSpawnedDataEvent;
        event Action PoolsPreparedEvent;
    }
}