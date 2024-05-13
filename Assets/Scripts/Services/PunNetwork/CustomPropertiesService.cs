using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Services.PunNetwork.Impls;
using UnityEngine;
using Zenject;
using static Utils.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Services.PunNetwork
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
        private IPlayersInRoomService _playersInRoomService;
        public event Action PlayersSpawnedEvent;

        [Inject]
        private void Construct
        (
            IPlayersInRoomService playersInRoomService
        )
        {
            _playersInRoomService = playersInRoomService;
        }

        private void Awake() => DontDestroyOnLoad(this);

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
                case PlayerProperty.PlayerNumber:
                    Debug.Log($"Player:{player.ActorNumber}, PlayerNumber changed to {value}");
                    break;
                case PlayerProperty.IsSpawned:
                    if (_playersInRoomService.IsAllReady())
                        _playersInRoomService.OnAllSpawned();
                    break;
                default:
                    Debug.LogWarning($"Unknown property changed: {key}");
                    break;
            }
        }

    }
}