using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Services.Data;
using UnityEngine;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace PunNetwork.Services.CustomProperties
{
public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
{
    public event Action<Player, bool> PlayerSpawnedEvent;
    public event Action<Player, bool> PoolsPreparedEvent;
    public event Action<Player, float> PlayerHealthPointsChangedEvent;
    public event Action<Player, PlayerSpawnedData> GetPlayerSpawnedDataEvent;

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
            case PlayerProperty.IsPlayersSpawned:
                var isPlayersSpawned = Convert.ToBoolean(value);
                Debug.Log($"Player spawned {player.ActorNumber} Key: {key}, Value: {isPlayersSpawned}");
                PlayerSpawnedEvent?.Invoke(player, isPlayersSpawned);
                break;
            case PlayerProperty.IsPoolsPrepared:
                var isPoolsPrepared = Convert.ToBoolean(value);
                Debug.Log($"Player pools prepared {player.ActorNumber} Key: {key}, Value: {isPoolsPrepared}");
                PoolsPreparedEvent?.Invoke(player, isPoolsPrepared);
                break;
            case PlayerProperty.PlayerSpawnedData:
                var resultData = JsonConvert.DeserializeObject<PlayerSpawnedData>(value.ToString());
                GetPlayerSpawnedDataEvent?.Invoke(player, resultData);
                break;
            case PlayerProperty.PlayerHP:
                var playerHealthPoints = Convert.ToSingle(value);
                Debug.Log($"Player:{player.ActorNumber}, PlayerHP changed to {playerHealthPoints}");
                PlayerHealthPointsChangedEvent?.Invoke(player, playerHealthPoints);
                break;
            default:
                Debug.LogWarning($"Unknown property changed: {key}");
                break;
        }
    }
}
}