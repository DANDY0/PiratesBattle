using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Services.PunNetwork.Impls;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.PunNetwork
{
    public class PlayersInRoomService : IPlayersInRoomService, IInitializable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private List<PlayerView> _playerViews = new();

        public PlayersInRoomService(ICustomPropertiesService customPropertiesService)
        {
            _customPropertiesService = customPropertiesService;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerSpawnedEvent += PlayerSpawnedHandler;
        }

        public bool IsAllReady()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.CustomProperties.TryGetValue(Enumerators.PlayerProperty.IsSpawned.ToString(), out var isSpawned))
                {
                    if ((bool)isSpawned) continue;
                    isAllReady = false;
                    break;
                }
                else
                    isAllReady = false;

            return isAllReady;
        }

        public void OnAllSpawned()
        {
            var playerViews = Object.FindObjectsOfType<PlayerView>();
            Debug.Log($"OnAllSpawned: {playerViews}");
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

        private void PlayerSpawnedHandler()
        {
            var isAllReady = IsAllReady();
            Debug.Log($"Player spawned, IsAllReady:{isAllReady}");
            if (isAllReady)
                OnAllSpawned();
        }
    }
}