using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.NetworkData;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.RoomPlayer;
using Utils;
using Utils.Extensions;
using Zenject;

namespace PunNetwork.Services.PlayersStats
{
    public class PlayersStatsService : IPlayersStatsService, IInitializable, IDisposable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IRoomPlayerService _roomPlayerService;
        private readonly Dictionary<Player, StatsValuesVo> _playersStats = new();

        public PlayersStatsService
        (
            ICustomPropertiesService customPropertiesService,
            IRoomPlayerService roomPlayerService
        )
        {
            _customPropertiesService = customPropertiesService;
            _roomPlayerService = roomPlayerService;
        }

        public void Initialize()
        {
            foreach (var player in _roomPlayerService.Players)
                _playersStats.Add(player, _roomPlayerService.GetPlayerInfo(player).ImmutableDataVo.InitialStats);

            SendPersonalInitialStats();

            _customPropertiesService.Subscribe(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
        }

        public void Dispose()
        {
            _customPropertiesService.Unsubscribe(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
        }

        public void SendPlayerHp(float healthPoints)
        {
            _playersStats[PhotonNetwork.LocalPlayer].HealthPoints = healthPoints;
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP,
                healthPoints);
        }

        private void SendPersonalInitialStats()
        {
            var healthPoints = _playersStats[PhotonNetwork.LocalPlayer].HealthPoints;
            
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP,
                healthPoints);
        }

        private void OnPlayerHPChanged(Player player, object value)
        {
            if (player.IsLocal)
                return;
            if (value is float hp) 
                _playersStats[player].HealthPoints = hp;
        }
    }
}