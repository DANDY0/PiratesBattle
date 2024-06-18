using Controllers;
using Photon.Pun;
using PunNetwork.Services;
using PunNetwork.Services.GameNetwork;
using PunNetwork.Services.ProjectNetwork;
using Services.GamePools;
using States.Core;

namespace States
{
    public class PrepareGameState : IState
    {
        private readonly LoadingController _loadingController;
        private readonly IGameNetworkService _gameNetworkService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IGamePoolsService _gamePoolsService;

        public PrepareGameState
        (
            LoadingController loadingController,
            IGameNetworkService gameNetworkService,
            IProjectNetworkService projectNetworkService,
            IGamePoolsService gamePoolsService
        )
        {
            _loadingController = loadingController;
            _gameNetworkService = gameNetworkService;
            _projectNetworkService = projectNetworkService;
            _gamePoolsService = gamePoolsService;
        }

        public void Enter()
        {
            if (PhotonNetwork.IsMasterClient)
                _gamePoolsService.PreparePools();
            _gameNetworkService.Setup();
        }

        public void Exit()
        {
            _loadingController.Hide();
            _projectNetworkService.IsGameStarted = true;
        }
    }
}