using System;
using System.Collections.Generic;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.MenuNetwork;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Views.Player;
using Zenject;

namespace Services.RoomPlayers
{
    public class RoomPlayersServices: IRoomPlayersServices, IInitializable, IDisposable
    {
        private readonly IMenuNetworkService _menuNetworkService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly ICustomPropertiesService _customPropertiesService;
        private Dictionary<Player, PlayerView> _playerViews;

        public RoomPlayersServices
        (
            IMenuNetworkService menuNetworkService,
            IProjectNetworkService projectNetworkService,
            ICustomPropertiesService customPropertiesService
        )
        {
            _menuNetworkService = menuNetworkService;
            _projectNetworkService = projectNetworkService;
            _customPropertiesService = customPropertiesService;
        }

        public void Initialize()
        {
            _projectNetworkService.ConnectedToMasterEvent += ConnectedToMasterHandler;
        }

        private void ConnectedToMasterHandler()
        {
            _menuNetworkService.RoomFilledEvent += RoomFilledHandler;
            _customPropertiesService.
        }

        private void RoomFilledHandler()
        {
            _playerViews = new Dictionary<Player, PlayerView>();
        }

        public void Dispose()
        {
            _menuNetworkService.RoomFilledEvent -= RoomFilledHandler;
        }
    }
}