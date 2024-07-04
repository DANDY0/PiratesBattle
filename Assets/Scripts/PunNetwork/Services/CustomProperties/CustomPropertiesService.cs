using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;
using Logger = Utils.Logger;

namespace PunNetwork.Services.CustomProperties
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
        /*public event Action<Player, bool> PlayerSpawnedEvent;
        public event Action<Player, bool> PoolsPreparedEvent;
        public event Action<Player, ReadyPlayerInfo> GetReadyPlayerInfoEvent;
        public event Action<Player, float> PlayerHealthPointsChangedEvent;*/

        private readonly Dictionary<PlayerProperty, List<Action<Player, object>>> _eventSubscriptions = new();

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (var entry in changedProps)
            {
                if (!Enum.TryParse(entry.Key.ToString(), out PlayerProperty propKey)) continue;
                var propValue = entry.Value;
                Logger.Log(this, $"OnPlayer {targetPlayer.ActorNumber}, Property {propKey} was updated to {propValue}");
                HandlePropertyChange(propKey, propValue, targetPlayer);
            }
        }

        public void Subscribe(PlayerProperty property, Action<Player, object> handler)
        {
            if (!_eventSubscriptions.ContainsKey(property)) 
                _eventSubscriptions[property] = new List<Action<Player, object>>();

            _eventSubscriptions[property].Add(handler);
        }

        public void Unsubscribe(PlayerProperty property, Action<Player, object> handler)
        {
            if (!_eventSubscriptions.TryGetValue(property, out var subscription)) 
                return;
            subscription.Remove(handler);
            if (_eventSubscriptions[property].Count == 0) 
                _eventSubscriptions.Remove(property);
        }

        private void HandlePropertyChange(PlayerProperty key, object value, Player player)
        {
            if (!_eventSubscriptions.TryGetValue(key, out var subscribers)) return;
            foreach (var subscriber in subscribers)
                subscriber?.Invoke(player, value);

            /*switch (key)
            {
                case PlayerProperty.AllSpawnedLocally:
                    var isPlayersSpawned = Convert.ToBoolean(value);
                    Debug.Log($"Player spawned {player.ActorNumber} Key: {key}, Value: {isPlayersSpawned}");
                    PlayerSpawnedEvent?.Invoke(player, isPlayersSpawned);
                    break;
                case PlayerProperty.AllPreparedPoolsLocally:
                    var isPoolsPrepared = Convert.ToBoolean(value);
                    Debug.Log($"Player pools prepared {player.ActorNumber} Key: {key}, Value: {isPoolsPrepared}");
                    PoolsPreparedEvent?.Invoke(player, isPoolsPrepared);
                    break;
                case PlayerProperty.ReadyData:
                    var resultData = JsonConvert.DeserializeObject<ReadyPlayerInfo>(value.ToString());
                    GetReadyPlayerInfoEvent?.Invoke(player, resultData);
                    break;
                case PlayerProperty.PlayerHP:
                    var playerHealthPoints = Convert.ToSingle(value);
                    Debug.Log($"Player:{player.ActorNumber}, PlayerHP changed to {playerHealthPoints}");
                    PlayerHealthPointsChangedEvent?.Invoke(player, playerHealthPoints);
                    break;
                default:
                    if (_eventSubscriptions.TryGetValue(key, out var subscribers))
                        foreach (var subscriber in subscribers)
                            subscriber?.Invoke(player, value);
                    break;
            }*/
        }
    }
}