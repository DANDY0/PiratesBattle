using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Services.PunNetwork
{
    public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private TeamMarker _teamMarker;
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            info.Sender.SetCustomProperties(new Hashtable { { Enumerators.PlayerProperty.IsSpawned.ToString(), true } });
        }

        public void SetTeamMarker(Enumerators.TeamRole role)
        {
            _teamMarker.SetView(role);
        }
    }
}