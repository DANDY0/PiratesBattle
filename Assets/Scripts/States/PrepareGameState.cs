using Controllers;
using PunNetwork.Services;
using PunNetwork.Services.Impls;
using States.Core;

namespace States
{
    public class PrepareGameState : IState
    {
        private readonly LoadingController _loadingController;
        private readonly IGameNetworkService _gameNetworkService;
        private readonly IProjectNetworkService _projectNetworkService;

        public PrepareGameState
        (
            LoadingController loadingController,
            IGameNetworkService gameNetworkService,
            IProjectNetworkService projectNetworkService
        )
        {
            _loadingController = loadingController;
            _gameNetworkService = gameNetworkService;
            _projectNetworkService = projectNetworkService;
        }
        
        public void Enter()
        {
            _gameNetworkService.Setup();
        }

        public void Exit()
        {
            _loadingController.Hide();
            _projectNetworkService.IsGameStarted = true;
        }
    }
}