using Controllers;
using Enums;
using PunNetwork.Services;
using PunNetwork.Services.ProjectNetwork;
using Services.Window;
using States.Core;

namespace States
{
    public class GameplayState : IState
    {
        private readonly MatchInfoController _matchInfoController;
        private readonly IWindowService _windowService;
        private readonly IProjectNetworkService _projectNetworkService;

        public GameplayState
        (
            MatchInfoController matchInfoController,
            IWindowService windowService,
            IProjectNetworkService projectNetworkService
        )
        {
            _matchInfoController = matchInfoController;
            _windowService = windowService;
            _projectNetworkService = projectNetworkService;
        }
        
        public void Enter()
        {
            _matchInfoController.Show();
        }

        public void Exit()
        {
            _windowService.Close(EWindow.MatchInfo);
            _projectNetworkService.IsGameStarted = false;
        }
    }
}