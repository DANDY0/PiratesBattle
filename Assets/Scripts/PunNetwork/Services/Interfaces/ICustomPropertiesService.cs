using System;
using PunNetwork.NetworkData;
using Utils;

namespace PunNetwork.Services
{
    public interface ICustomPropertiesService
    {
        public event Action PlayerSpawnedEvent;
        event Action PlayerLivesChangedEvent;
        public event Action<NetworkDataModel.PlayerSpawnedData> GetPlayerSpawnedDataEvent;
    }
}