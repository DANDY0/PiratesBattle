using System.Linq;
using Controllers;
using PunNetwork.Services.RoomPlayer;
using Services.GameplayService;
using Services.Input;
using States.Core;

namespace States
{
    public class GameplayState : IState
    {
        private readonly MatchInfoController _matchInfoController;
        private readonly IInputService _inputService;
        private readonly IRoomPlayersService _roomPlayersService;
        private readonly IGameplayService _gameplayService;

        public GameplayState
        (
            MatchInfoController matchInfoController,
            IInputService inputService,
            IRoomPlayersService roomPlayersService,
            IGameplayService gameplayService
        )
        {
            _matchInfoController = matchInfoController;
            _inputService = inputService;
            _roomPlayersService = roomPlayersService;
            _gameplayService = gameplayService;
        }

        public void Enter()
        {
            _gameplayService.Activate();
            _inputService.Activate();
            
            var playerViews = _roomPlayersService.Players.Select(p
                => _roomPlayersService.GetPlayerInfo(p).View);
            
            foreach (var playerView in playerViews)
                playerView.SubscribeOnInput();
            
            _matchInfoController.Show();
        }

        public void Exit()
        {
        }
    }
}