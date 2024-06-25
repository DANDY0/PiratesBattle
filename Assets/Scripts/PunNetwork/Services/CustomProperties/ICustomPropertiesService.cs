using System;
using Photon.Realtime;
using PunNetwork.NetworkData;

namespace PunNetwork.Services.CustomProperties
{
    public interface ICustomPropertiesService 
    {
        event Action<Player, bool> PlayerSpawnedEvent;
        event Action<Player, bool> PoolsPreparedEvent;
        event Action<Player, float> PlayerHealthPointsChangedEvent;
        event Action<Player, NetworkDataModel.ReadyPlayerInfo> GetReadyPlayerInfoEvent;
    }
}