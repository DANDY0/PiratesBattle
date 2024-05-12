using System;
using Photon.Pun;
using Zenject;

namespace Services.PunNetwork
{
    public class PlayersInRoomService: MonoBehaviourPunCallbacks, IPlayersInRoomService
    {
        private ICustomPropertiesService _customPropertiesService;

        [Inject]
        private void Construct
        (
            ICustomPropertiesService customPropertiesService
        )
        {
            _customPropertiesService = customPropertiesService;
        }

        private void Awake()
        {
            _customPropertiesService.PlayersSpawnedEvent += PlayersSpawnedHandler;
        }

        private void PlayersSpawnedHandler()
        {
           
        }
    }
}