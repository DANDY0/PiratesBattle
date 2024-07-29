using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Services.SpawnPoints;
using UnityEngine;

namespace PunNetwork.Services.SpawnPlayer
{
    public class SpawnPlayerService : ISpawnPlayerService
    {
        private readonly IRoomPlayersService _roomPlayersService;
        private readonly ISpawnPointsHandler _spawnPointsHandler;

        public SpawnPlayerService
        (
            IRoomPlayersService roomPlayersService,
            ISpawnPointsHandler spawnPointsHandler
        )
        {
            _roomPlayersService = roomPlayersService;
            _spawnPointsHandler = spawnPointsHandler;
        }

        public void SpawnPlayer()
        {
            var photonTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            var playerPosition =
                _spawnPointsHandler.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, photonTeam);

            PhotonNetwork.Instantiate(
                _roomPlayersService.GetPlayerInfo(PhotonNetwork.LocalPlayer).ImmutableDataVo.CharacterName, playerPosition,
                Quaternion.identity);
        }
    }
}