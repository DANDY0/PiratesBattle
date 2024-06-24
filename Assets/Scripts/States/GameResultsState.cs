using Controllers;
using Enums;
using PunNetwork.Services.GameNetwork;
using Services.Window;
using States.Core;

namespace States
{
    public class GameResultsState : IState
    {
        private readonly MatchResultsController _matchResultsController;
        private readonly IGameNetworkService _gameNetworkService;
        private readonly IWindowService _windowService;

        public GameResultsState
        (
            MatchResultsController matchResultsController,
            IGameNetworkService gameNetworkService,
            IWindowService windowService
        )
        {
            _matchResultsController = matchResultsController;
            _gameNetworkService = gameNetworkService;
            _windowService = windowService;
        }
        
        public void Enter()
        {
            _windowService.Close(EWindow.MatchInfo);
            _matchResultsController.Show(_gameNetworkService.GameResult);
        }

        public void Exit()
        {
            
        }
    }
}