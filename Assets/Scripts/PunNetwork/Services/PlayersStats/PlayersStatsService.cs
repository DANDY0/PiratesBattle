using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.RoomPlayer;
using Utils;
using Utils.Extensions;
using Zenject;
using static PunNetwork.NetworkData.NetworkDataModel;

namespace PunNetwork.Services.PlayersStats
{
    public class PlayersStatsService : IPlayersStatsService, IInitializable, IDisposable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IRoomPlayersService _roomPlayersService;
        public readonly Dictionary<Player, StatsValuesVo> PlayersStats = new();

        public PlayersStatsService
        (
            ICustomPropertiesService customPropertiesService,
            IRoomPlayersService roomPlayersService
        )
        {
            _customPropertiesService = customPropertiesService;
            _roomPlayersService = roomPlayersService;
        }

        public void Initialize()
        {
            foreach (var player in _roomPlayersService.Players)
                PlayersStats.Add(player, _roomPlayersService.GetPlayerInfo(player).ImmutableDataVo.InitialStats);

            SendPersonalInitialStats();

            _customPropertiesService.Subscribe<float>(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
            _customPropertiesService.Subscribe<bool>(Enumerators.PlayerProperty.IsDead, OnPlayerDead);
        }

        public void Dispose()
        {
            _customPropertiesService.Unsubscribe<float>(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
            _customPropertiesService.Unsubscribe<bool>(Enumerators.PlayerProperty.IsDead, OnPlayerDead);
        }

        public void SendPlayerHp(float healthPoints)
        {
            PlayersStats[PhotonNetwork.LocalPlayer].HealthPoints = healthPoints;
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP, healthPoints);
            
            if(healthPoints <= 0)
                SendPlayerDead();
        }

        private void SendPersonalInitialStats()
        {
            var healthPoints = PlayersStats[PhotonNetwork.LocalPlayer].HealthPoints;
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP, healthPoints);
        }

        private void SendPlayerDead()
        {
            PlayersStats[PhotonNetwork.LocalPlayer].IsDead = true;
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.IsDead, true);
        }

        private void OnPlayerHPChanged(Player player, float hp)
        {
            if (player.IsLocal)
                return;

            PlayersStats[player].HealthPoints = hp;
        }

        private void OnPlayerDead(Player player, bool state)
        {
            if (player.IsLocal)
                return;

            PlayersStats[player].IsDead = state;
        }
    }

}