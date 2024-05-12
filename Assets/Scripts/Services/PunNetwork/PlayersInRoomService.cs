using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Services.PunNetwork.Impls;
using UnityEngine;
using Utils;

namespace Services.PunNetwork
{
    public class PlayersInRoomService : IPlayersInRoomService
    {
        private List<PlayerView> _playerViews = new();

        public void CheckIfAllSpawned()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.CustomProperties.TryGetValue(Enumerators.PlayerProperty.IsSpawned.ToString(),
                        out var isSpawned))
                {
                    if ((bool)isSpawned) continue;
                    isAllReady = false;
                    break;
                }

            if (isAllReady)
            {
                var playerViews = Object.FindObjectsOfType<PlayerView>();
                foreach (var playerView in playerViews)
                {
                    _playerViews.Add(playerView);
                    
                    
                    var player = playerView.GetComponent<PhotonView>().Owner;

                    Enumerators.TeamRole teamRole;
                    if (player.IsLocal)
                        teamRole = Enumerators.TeamRole.MyPlayer;
                    else
                        teamRole = player.GetPhotonTeam().Code == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                            ? Enumerators.TeamRole.AllyPlayer
                            : Enumerators.TeamRole.EnemyPlayer;

                    playerView.SetTeamMarker(teamRole);
                }
            }
        }
    }
}