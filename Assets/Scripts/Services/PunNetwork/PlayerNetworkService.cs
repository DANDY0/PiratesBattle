using System;
using Photon.Pun;
using Services.PunNetwork.Impls;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.PunNetwork
{
    public class PlayerNetworkService: MonoBehaviourPunCallbacks, IPlayerNetworkService
    {
        private PlayerSpawner _playerSpawner;
        private ICustomPropertiesService _customPropertiesService;

        [Inject]
        private void Construct
        (
            ICustomPropertiesService customPropertiesService
        )
        {
            _customPropertiesService = customPropertiesService;
        }
        
        private void Start()
        {
            _playerSpawner = new PlayerSpawner();
        }

        public void SpawnOurPlayer()
        {
            _playerSpawner.SpawnPlayer();
        }
    }
    
    public class PlayerSpawner
    {

        public void SpawnPlayer()
        {
            PhotonNetwork.Instantiate("TeamPlayers\\" + Enumerators.TeamRole.MyPlayer, Vector3.zero, Quaternion.identity);
        }
    }

}