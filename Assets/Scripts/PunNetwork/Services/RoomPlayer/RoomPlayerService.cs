using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.MasterEvent;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Views.Player;
using Services.Pool;
using Utils.Extensions;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;


namespace PunNetwork.Services.RoomPlayer
{
    public class RoomPlayerService : IRoomPlayerService, IDisposable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IMasterEventService _masterEventService;
        private readonly IPoolService _poolService;

        private readonly Dictionary<Player, PlayerInfoVo> _playersDictionary = new();
        
        private bool IsAllPlayersSpawned 
        {
            get
            {
                return _playersDictionary.Values.All(info => info.IsLocalPlayersSpawned);
            }
        }
        
        private bool IsAllPoolsPrepared 
        {
            get
            {
                return _playersDictionary.Values.All(info => info.IsLocalPoolsPrepared);
            }
        }

        private bool IsAllPlayersSpawnedLocal
        {
            get
            {
                return _playersDictionary.Values.All(info => info.View != null);
            }
        }
        
        private bool IsAllDataGet
        {
            get
            {
                return _playersDictionary.Values.All(info => info.ImmutableDataVo != null);
            }
        }

        public IEnumerable<Player> Players => _playersDictionary.Keys;


        public RoomPlayerService
        (
            ICustomPropertiesService customPropertiesService,
            IProjectNetworkService projectNetworkService,
            IMasterEventService masterEventService
        )
        {
            _customPropertiesService = customPropertiesService;
            _projectNetworkService = projectNetworkService;
            _masterEventService = masterEventService;
        }
        
        public void Dispose()
        {
            _projectNetworkService.PlayerLeftRoomEvent -= LeaveRoom;

            _customPropertiesService.Unsubscribe(PlayerProperty.PlayerImmutableData, OnPlayerImmutableDataChanged);
            _customPropertiesService.Unsubscribe(PlayerProperty.LocalPlayersSpawned, OnLocalPlayersSpawnedChanged);
            _customPropertiesService.Unsubscribe(PlayerProperty.LocalPoolsPrepared, OnLocalPoolsPreparedChanged);
        }

        public PlayerInfoVo GetPlayerInfo(Player player)
        {
            try
            {
                return _playersDictionary[player];
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"[{nameof(RoomPlayerService)}] Player with name {player} was not present in the dictionary. {e.StackTrace}");
            }
        }
        
        public void EnterRoom(IEnumerable<Player> players)
        {
            foreach (var player in players) 
                _playersDictionary.Add(player, new PlayerInfoVo());
            
            _customPropertiesService.Subscribe(PlayerProperty.PlayerImmutableData, OnPlayerImmutableDataChanged);
            _customPropertiesService.Subscribe(PlayerProperty.LocalPlayersSpawned, OnLocalPlayersSpawnedChanged);
            _customPropertiesService.Subscribe(PlayerProperty.LocalPoolsPrepared, OnLocalPoolsPreparedChanged);

            _projectNetworkService.EnterRoom();
            _projectNetworkService.PlayerLeftRoomEvent += LeaveRoom;

            _masterEventService.Setup();
        }

        public void SendPlayerImmutableData(PlayerImmutableDataVo data)
        {
            var json = JsonConvert.SerializeObject(data);
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.PlayerImmutableData, json);
        }

        public void SendLocalPlayersSpawned(Player player, PlayerView playerView)
        {
            var teamRole = player.GetTeamRole();
            playerView.SetTeamRole(teamRole);

            _playersDictionary[player].View = playerView;
            _playersDictionary[player].TeamRole = teamRole;

            if (IsAllPlayersSpawnedLocal) 
                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.LocalPlayersSpawned, true);
        }

        public void SendLocalPoolsPrepared() => 
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.LocalPoolsPrepared, true);
        
        private void LeaveRoom(Player player)
        {
            if (player.IsLocal)
            {
                _projectNetworkService.PlayerLeftRoomEvent -= LeaveRoom;

                _playersDictionary.Clear();

                _customPropertiesService.Unsubscribe(PlayerProperty.PlayerImmutableData, OnPlayerImmutableDataChanged);
                _customPropertiesService.Unsubscribe(PlayerProperty.LocalPlayersSpawned, OnLocalPlayersSpawnedChanged);
                _customPropertiesService.Unsubscribe(PlayerProperty.LocalPoolsPrepared, OnLocalPoolsPreparedChanged);
            }
            else
                _playersDictionary.Remove(player);
        }

        private void OnPlayerImmutableDataChanged(Player player, object value)
        {
            _playersDictionary[player].ImmutableDataVo = JsonConvert.DeserializeObject<PlayerImmutableDataVo>(value.ToString());
            
            if (IsAllDataGet) 
                _masterEventService.OnAllDataGet();
        }

        private void OnLocalPlayersSpawnedChanged(Player player, object value)
        {
            if (value is bool isLocalPlayersSpawned) 
                _playersDictionary[player].IsLocalPlayersSpawned = isLocalPlayersSpawned;
            
            if (IsAllPlayersSpawned) 
                _masterEventService.OnAllPlayersSpawned();
        }

        private void OnLocalPoolsPreparedChanged(Player player, object value)
        {
            if (value is bool isLocalPoolsPrepared) 
                _playersDictionary[player].IsLocalPoolsPrepared = isLocalPoolsPrepared;

            if (IsAllPoolsPrepared) 
                _masterEventService.OnAllPoolsPrepared();
        }
    }
}