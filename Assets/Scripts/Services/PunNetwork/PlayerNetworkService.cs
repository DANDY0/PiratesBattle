using System;
using Photon.Pun;
using Services.PunNetwork.Impls;
using Services.SpawnPoints;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.PunNetwork
{
    public class PlayerNetworkService : IPlayerNetworkService
    {
        private readonly ISpawnPointsService _spawnPointsService;

        public PlayerNetworkService
        (
            ISpawnPointsService spawnPointsService
        )
        {
            _spawnPointsService = spawnPointsService;
        }


        public void SpawnOurPlayer()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate("TeamPlayers\\" + Enumerators.TeamRole.MyPlayer,
                _spawnPointsService.GetSpawnPoint(PhotonNetwork.PlayerList.Length,
                    PhotonNetwork.LocalPlayer.ActorNumber - 1), Quaternion.identity);
        }
    }
}