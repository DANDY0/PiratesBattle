using System;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Services.Data;
using UnityEngine;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace PunNetwork.Services.CustomProperties
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
        public event Action PlayerSpawnedEvent;
        public event Action PoolsPreparedEvent;
        public event Action PlayerLivesChangedEvent;
        public event Action<PlayerSpawnedData> GetPlayerSpawnedDataEvent;
        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (var entry in changedProps)
            {
                if (!Enum.TryParse(entry.Key.ToString(), out PlayerProperty propKey)) continue;
                var propValue = entry.Value;
                Debug.Log($"OnPlayer {targetPlayer.ActorNumber}, Property {propKey} was updated to {propValue}");
                HandlePropertyChange(propKey, propValue, targetPlayer);
            }
        }

        private void HandlePropertyChange(PlayerProperty key, object value, Player player)
        {
            switch (key)
            {
                case PlayerProperty.IsSpawned:
                    Debug.Log($"Player spawned {player.ActorNumber} Key: {key}");
                    PlayerSpawnedEvent?.Invoke();
                    break;
                case PlayerProperty.IsPoolsPrepared:
                    Debug.Log($"Player spawned {player.ActorNumber} Key: {key}");
                    PoolsPreparedEvent?.Invoke();
                    break;
                case PlayerProperty.PlayerSpawnedData:
                    PlayerSpawnedData resultData = JsonConvert.DeserializeObject<PlayerSpawnedData>(value.ToString());
                    GetPlayerSpawnedDataEvent?.Invoke(resultData);
                    break;
                case PlayerProperty.PlayerNumber:
                    Debug.Log($"Player:{player.ActorNumber}, PlayerNumber changed to {value}");
                    break;
                case PlayerProperty.PlayerLives:
                    PlayerLivesChangedEvent?.Invoke();
                    break;
                default:
                    Debug.LogWarning($"Unknown property changed: {key}");
                    break;
            }
        }

     
    }
}