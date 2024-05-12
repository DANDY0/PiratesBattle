using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Utils.Enumerators;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Services.PunNetwork
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
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
                    break;
                case PlayerProperty.PlayerNumber:
                    Debug.Log($"Player:{player.ActorNumber}, PlayerNumber changed to {value}");
                    break;
                default:
                    Debug.LogWarning($"Unknown property changed: {key}");
                    break;
            }
        }
    }
}