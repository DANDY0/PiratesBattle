using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Views.Player;
using Services.Data;
using Services.Pool;
using States;
using States.Core;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;

namespace PunNetwork.Services.ObjectsInRoom
{
    public class ObjectsInRoomService : IObjectsInRoomService, IInitializable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IDataService _dataService;
        private readonly IPoolService _poolService;

        private bool _isAllPlayersSpawned;
        private bool _isAllPoolsPrepared;

        private readonly Dictionary<Player, PlayerView> _playersDictionary = new();
        public List<PlayerView> PlayerViews { get; } = new();
        
        public ObjectsInRoomService
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
            _customPropertiesService.PoolsPreparedEvent += PoolsPreparedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent += GetPlayerSpawnedDataHandler;
        }

        public void Dispose()
        {
            _customPropertiesService.PlayerSpawnedEvent -= PlayerSpawnedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent -= GetPlayerSpawnedDataHandler;
            _customPropertiesService.PoolsPreparedEvent -= PoolsPreparedHandler;
        }

        public void OnPlayerSpawned(Player player, PlayerView playerView)
        {
            _playersDictionary.Add(player, playerView);
            PlayerViews.Add(playerView);
            
            SetTeamRole(playerView);

            if (_playersDictionary.Keys.Count == PhotonNetwork.CurrentRoom.PlayerCount) 
                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.IsPlayersSpawned, true);
        }
        
        public void PlayerLeftRoom(Player player)
        {
            _playersDictionary.Remove(player, out var playerView);
            PlayerViews.Remove(playerView);
        }

        private void PlayerSpawnedHandler(Player player, bool isSpawned)
        {
            if (!isSpawned)
                return;
            var isAllReady = IsAllReady();
            Debug.Log($"Player spawned, IsAllReady:{isAllReady}");
            if (isAllReady)
                OnAllSpawned();
        }

        private void PoolsPreparedHandler(Player player, bool isPrepared)
        {
            if (!isPrepared)
                return;
            var isAllReady = IsAllPreparedPools();
            if (isAllReady)
                OnAllPoolsPrepared();
        }

        private static bool IsAllReady()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.TryGetCustomProperty(PlayerProperty.IsPlayersSpawned, out var isSpawned))
                {
                    if ((bool)isSpawned) continue;
                    isAllReady = false;
                    break;
                }
                else
                    isAllReady = false;

            return isAllReady;
        }

        private static bool IsAllPreparedPools()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.TryGetCustomProperty(PlayerProperty.IsPoolsPrepared, out var isPrepared))
                {
                    if ((bool)isPrepared) continue;
                    isAllReady = false;
                    break;
                }
                else
                    isAllReady = false;

            return isAllReady;
        }

        private void OnAllSpawned()
        {
            _isAllPlayersSpawned = true;
            
            SendPlayerSpawnedData();

            GoNextState();
        }

        private void OnAllPoolsPrepared()
        {
            _isAllPoolsPrepared = true;
            GoNextState();
        }

        private void GoNextState()
        {
            if (!_isAllPlayersSpawned || !_isAllPoolsPrepared)
                return;

            if (_projectNetworkService.IsGameStarted)
                _gameStateMachine.Enter<GameplayState>();
            else
                _gameStateMachine.Enter<MatchPreviewState>();
        }
        
        public void UpdateHealthPoints(Player player, float newHealthPoints)
        {
            _playersDictionary[player].UpdateHealthPoints(newHealthPoints);
        }

        private void SendPlayerSpawnedData()
        {
            var playerSpawnedData = new PlayerSpawnedData(PhotonNetwork.LocalPlayer.ActorNumber,
                _dataService.CachedUserLocalData.NickName, _dataService.CachedUserLocalData.SelectedCharacter.ToString(),1);
            var json = JsonConvert.SerializeObject(playerSpawnedData);

            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.PlayerSpawnedData, json);
        }

        private static void SetTeamRole(PlayerView playerView)
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

        private void GetPlayerSpawnedDataHandler(Player player, PlayerSpawnedData playerSpawnedData)
        {
            _playersDictionary[player].SetNickname(playerSpawnedData.Nickname);
        }
    }
}