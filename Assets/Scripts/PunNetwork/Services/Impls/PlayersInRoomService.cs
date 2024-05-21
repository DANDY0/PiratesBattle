using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Views;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;
using static Utils.Enumerators;

namespace PunNetwork.Services.Impls
{
    public class PlayersInRoomService : IPlayersInRoomService, IInitializable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly List<PlayerView> _playerViews = new();

        public PlayersInRoomService
        (
            ICustomPropertiesService customPropertiesService
        )
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
                if (player.CustomProperties.TryGetValue(PlayerProperty.IsSpawned.ToString(), out var isSpawned))
                {
                    if ((bool)isSpawned) continue;
                    isAllReady = false;
                    break;
                }
                else
                    isAllReady = false;

            return isAllReady;
        }

        public bool IsAllEnemiesDestroyed()
        {
            var allDestroyed = true;

            var enemies = _playerViews.Where(p => p.TeamRole == TeamRole.EnemyPlayer);
            foreach (var p in enemies)
            {
                if (p.PhotonView.Owner.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives) && (int)lives <= 0) 
                    continue;
                allDestroyed = false;
                break;
            }
            
            return allDestroyed;
        }

        public void UpdateHearts()
        {
            foreach (var playerView in _playerViews)
            {
                playerView.PhotonView.Owner.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives);
                playerView.UpdateHearts((int)lives);
            }
        }

        public void OnAllSpawned()
        {
            var playerViews = Object.FindObjectsOfType<PlayerView>();
            Debug.Log($"OnAllSpawned: {playerViews}");
            foreach (var playerView in playerViews)
            {
                _playerViews.Add(playerView);


                var player = playerView.PhotonView.Owner;

                TeamRole teamRole;
                if (player.IsLocal)
                    teamRole = TeamRole.MyPlayer;
                else
                    teamRole = player.GetPhotonTeam().Code == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                        ? TeamRole.AllyPlayer
                        : TeamRole.EnemyPlayer;

                playerView.SetTeamRole(teamRole);
                playerView.IsSpawnedOnServer = true;
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