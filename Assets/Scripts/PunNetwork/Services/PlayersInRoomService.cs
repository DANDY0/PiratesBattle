using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Views;
using Services.Data;
using States;
using States.Core;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;
using Object = UnityEngine.Object;

namespace PunNetwork.Services.Impls
{
    public class PlayersInRoomService : IPlayersInRoomService, IInitializable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;
        
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IDataService _dataService;
        
        private readonly List<PlayerView> _playerViews = new();

        public PlayersInRoomService
        (
            ICustomPropertiesService customPropertiesService,
            IGameStateMachine gameStateMachine,
            IProjectNetworkService projectNetworkService,
            IDataService dataService
        )
        {
            _customPropertiesService = customPropertiesService;
            _gameStateMachine = gameStateMachine;
            _projectNetworkService = projectNetworkService;
            _dataService = dataService;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerSpawnedEvent += PlayerSpawnedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent += GetPlayerSpawnedDataHandler;

        }

        public void Dispose()
        {
            _customPropertiesService.PlayerSpawnedEvent -= PlayerSpawnedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent -= GetPlayerSpawnedDataHandler;
        }

        #region Checkers
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
        
        #endregion

        public void UpdateHearts()
        {
            foreach (var playerView in _playerViews)
            {
                playerView.PhotonView.Owner.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives);
                playerView.UpdateHearts((int)lives);
            }
        }

        private void SendPlayerSpawnedData()
        {
            PlayerSpawnedData playerSpawnedData = new PlayerSpawnedData(PhotonNetwork.LocalPlayer.ActorNumber,
                _dataService.CachedUserLocalData.NickName, 0,1);
            string json = JsonConvert.SerializeObject(playerSpawnedData);
            
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.PlayerSpawnedData, json);
        }

        private void SetTeamRole(PlayerView playerView)
        {
            var player = playerView.PhotonView.Owner;

            TeamRole teamRole;
            if (player.IsLocal)
                teamRole = TeamRole.MyPlayer;
            else
                teamRole = player.GetPhotonTeam().Code == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                    ? TeamRole.AllyPlayer
                    : TeamRole.EnemyPlayer;

            playerView.SetTeamRole(teamRole);
        }

        public void OnAllSpawned()
        {
            var playerViews = Object.FindObjectsOfType<PlayerView>();
            Debug.Log($"OnAllSpawned: {playerViews}");
            foreach (var playerView in playerViews)
            {
                _playerViews.Add(playerView);


                SetTeamRole(playerView);
                playerView.IsSpawnedOnServer = true;


                if (_projectNetworkService.IsGameStarted)
                    _gameStateMachine.Enter<GameplayState>();
                else
                    _gameStateMachine.Enter<MatchPreviewState>();
            }

            SendPlayerSpawnedData();
        }

        private void GetPlayerSpawnedDataHandler(PlayerSpawnedData playerSpawnedData)
        {
            var player = _playerViews.Find(p=>p.PhotonView.Owner.ActorNumber == playerSpawnedData.ActorNumber);
            player.SetNickname(playerSpawnedData.Nickname);
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