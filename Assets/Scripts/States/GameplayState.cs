using Controllers;
using PunNetwork.Services.RoomPlayer;
using Services.Input;
using States.Core;

namespace States
{
    public class GameplayState : IState
    {
        private readonly MatchInfoController _matchInfoController;
        private readonly IInputService _inputService;
        private readonly IRoomPlayerService _roomPlayerService;

        public GameplayState
        (
            MatchInfoController matchInfoController,
            IInputService inputService,
            IRoomPlayerService roomPlayerService
        )
        {
            _matchInfoController = matchInfoController;
            _inputService = inputService;
            _roomPlayerService = roomPlayerService;
        }

        public void Enter()
        {
            _inputService.Enable();
            foreach (var playerView in _roomPlayerService.PlayerViews)
                playerView.SubscribeOnInput();
            _matchInfoController.Show();
        }

        public void Exit()
        {
        }
    }
}