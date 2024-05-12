using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Utils.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Services.PunNetwork
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
        public event Action PlayersSpawnedEvent;
        
        public void SetPlayerProperty(PlayerProperty property, object value)
        {
            Hashtable props = new Hashtable
            {
                { property.ToString(), value }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (DictionaryEntry entry in changedProps)
            {
                if (System.Enum.TryParse(entry.Key.ToString(), out PlayerProperty propKey))
                {
                    object propValue = entry.Value;
                    Debug.Log($"Property {propKey} was updated to {propValue}");
                    HandlePropertyChange(propKey, propValue, targetPlayer);
                }
            }
        }

        private void HandlePropertyChange(PlayerProperty key, object value, Player player)
        {
            switch (key)
            {
                case PlayerProperty.IsSpawned:
                    Debug.Log($"Player:{player.ActorNumber}, IsSpawned changed to {value}");
                    if (CheckIsAllSpawned()) 
                        PlayersSpawnedEvent?.Invoke();
                    break;
                case PlayerProperty.PlayerNumber:
                    Debug.Log($"Player:{player.ActorNumber}, PlayerNumber changed to {value}");
                    break;
                default:
                    Debug.LogWarning($"Unknown property changed: {key}");
                    break;
            }
        }

        private bool CheckIsAllSpawned()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                object isSpawned;
                if (player.CustomProperties.TryGetValue(PlayerProperty.IsSpawned.ToString(), out isSpawned))
                {
                    if (!(bool)isSpawned)
                        return false;
                }
                else
                    return false;
            }
            return true;
        }
    }
}