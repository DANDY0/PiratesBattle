using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Services.SpawnPoints;
using UnityEngine;

namespace PunNetwork.Services.SpawnPlayer
{
    public class SpawnPlayerService : ISpawnPlayerService
    {
        private readonly IRoomPlayerService _roomPlayerService;
        private readonly ISpawnPointsService _spawnPointsService;

        public SpawnPlayerService
        (
            IRoomPlayerService roomPlayerService,
            ISpawnPointsService spawnPointsService
        )
        {
            _roomPlayerService = roomPlayerService;
            _spawnPointsService = spawnPointsService;
        }

        public void SpawnPlayer()
        {
            var photonTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            var playerPosition =
                _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, photonTeam);

            PhotonNetwork.Instantiate(
                _roomPlayerService.GetPlayerInfo(PhotonNetwork.LocalPlayer).ImmutableDataVo.CharacterName, playerPosition,
                Quaternion.identity);
        }
    }
}