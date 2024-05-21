using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Utils;

namespace PunNetwork.Services.Impls
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
                _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, 
                    PhotonNetwork.LocalPlayer.GetPhotonTeam()), Quaternion.identity);
        }
    }
}