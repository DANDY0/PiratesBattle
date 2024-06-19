using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.PhotonUnityNetworking.Code.Common.Pool;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Views.Player;
using Services.Data;
using Services.GamePools;
using States;
using States.Core;
using UnityEngine;
using Utils.Extensions;
using Zenject;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;
using Object = UnityEngine.Object;

namespace PunNetwork.Services.ObjectsInRoom
{
    public class ObjectsInRoomService : IObjectsInRoomService, IInitializable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;

        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IDataService _dataService;
        private readonly IGamePoolsService _gamePoolsService;
        private readonly IPoolService _poolService;

        private bool _isAllPlayersSpawned;
        private bool _isAllPoolsPreparedAndSynced;

        public List<PlayerView> PlayerViews { get; } = new();

        public ObjectsInRoomService
        (
            ICustomPropertiesService customPropertiesService,
            IGameStateMachine gameStateMachine,
            IProjectNetworkService projectNetworkService,
            IDataService dataService,
            IGamePoolsService gamePoolsService
        )
        {
            _customPropertiesService = customPropertiesService;
            _gameStateMachine = gameStateMachine;
            _projectNetworkService = projectNetworkService;
            _dataService = dataService;
            _gamePoolsService = gamePoolsService;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerSpawnedEvent += PlayerSpawnedHandler;
            _customPropertiesService.PoolsPreparedEvent += PoolsPreparedHandler;
            _customPropertiesService.PoolsPreparedAndSyncedEvent += PoolsPreparedAndSyncedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent += GetPlayerSpawnedDataHandler;
        }

        public void Dispose()
        {
            _customPropertiesService.PlayerSpawnedEvent -= PlayerSpawnedHandler;
            _customPropertiesService.GetPlayerSpawnedDataEvent -= GetPlayerSpawnedDataHandler;
            _customPropertiesService.PoolsPreparedEvent -= PoolsPreparedHandler;
            _customPropertiesService.PoolsPreparedAndSyncedEvent += PoolsPreparedAndSyncedHandler;
        }

        public bool IsAllEnemiesDestroyed()
        {
            var allDestroyed = true;

            var enemies = PlayerViews.Where(p => p.TeamRole == TeamRole.EnemyPlayer);
            foreach (var p in enemies)
            {
                if (p.PhotonView.Owner.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives) &&
                    (int)lives <= 0)
                    continue;
                allDestroyed = false;
                break;
            }

            return allDestroyed;
        }

        private void PlayerSpawnedHandler()
        {
            var isAllReady = IsAllReady();
            Debug.Log($"Player spawned, IsAllReady:{isAllReady}");
            if (isAllReady)
                OnAllSpawned();
        }

        private void PoolsPreparedHandler()
        {
            var isAllReady = IsAllPreparedPools();
            if (isAllReady)
                OnAllPoolsPrepared();
        }

        private void PoolsPreparedAndSyncedHandler()
        {
            var isAllReady = IsAllPreparedAndSyncedPools();
            if (isAllReady)
                OnAllPoolsPreparedAndSynced();
        }

        private static bool IsAllReady()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.TryGetCustomProperty(PlayerProperty.IsSpawned, out var isSpawned))
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

        private static bool IsAllPreparedAndSyncedPools()
        {
            var isAllReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
                if (player.TryGetCustomProperty(PlayerProperty.IsPoolsPreparedAndSynced, out var isPrepared))
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
            var playerViews = Object.FindObjectsOfType<PlayerView>();
            Debug.Log($"OnAllSpawned: {playerViews}");
            foreach (var playerView in playerViews)
            {
                PlayerViews.Add(playerView);


                SetTeamRole(playerView);
                playerView.IsSpawnedOnServer = true;
            }

            SendPlayerSpawnedData();

            GoNextState();
        }

        private void OnAllPoolsPrepared()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var customProperties = new ExitGames.Client.Photon.Hashtable();

            foreach (var pool in _gamePoolsService.PhotonViewIdsDictionary)
                customProperties[pool.Key] = pool.Value;

            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }

        private void OnAllPoolsPreparedAndSynced()
        {
            _isAllPoolsPreparedAndSynced = true;
            GoNextState();
        }

        private void GoNextState()
        {
            if (!_isAllPlayersSpawned || !_isAllPoolsPreparedAndSynced)
                return;

            if (_projectNetworkService.IsGameStarted)
                _gameStateMachine.Enter<GameplayState>();
            else
                _gameStateMachine.Enter<MatchPreviewState>();
        }

        public void UpdateHearts()
        {
            foreach (var playerView in PlayerViews)
            {
                playerView.PhotonView.Owner.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives);
                playerView.UpdateHearts((int)lives);
            }
        }

        private void SendPlayerSpawnedData()
        {
            var playerSpawnedData = new PlayerSpawnedData(PhotonNetwork.LocalPlayer.ActorNumber,
                _dataService.CachedUserLocalData.NickName, 0, 1);
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

        private void GetPlayerSpawnedDataHandler(PlayerSpawnedData playerSpawnedData)
        {
            var player = PlayerViews.Find(p => p.PhotonView.Owner.ActorNumber == playerSpawnedData.ActorNumber);
            player.SetNickname(playerSpawnedData.Nickname);
        }
    }
}