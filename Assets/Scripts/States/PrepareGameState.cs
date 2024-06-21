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
        private readonly IPhotonPoolService _photonPoolService;

        public PrepareGameState
        (
            LoadingController loadingController,
            IGameNetworkService gameNetworkService,
            IProjectNetworkService projectNetworkService,
            IPhotonPoolService photonPoolService
        )
        {
            _loadingController = loadingController;
            _gameNetworkService = gameNetworkService;
            _projectNetworkService = projectNetworkService;
            _photonPoolService = photonPoolService;
        }

        public void Enter()
        {
            _photonPoolService.PreparePools();
            _gameNetworkService.Setup();
        }

        public void Exit()
        {
            _loadingController.Hide();
            _projectNetworkService.IsGameStarted = true;
        }
    }
}