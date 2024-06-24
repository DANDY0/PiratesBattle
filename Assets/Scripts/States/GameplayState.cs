using Controllers;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.ProjectNetwork;
using Services.Input;
using States.Core;

namespace States
{
    public class GameplayState : IState
    {
        private readonly MatchInfoController _matchInfoController;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IInputService _inputService;
        private readonly IObjectsInRoomService _objectsInRoomService;

        public GameplayState
        (
            MatchInfoController matchInfoController,
            IProjectNetworkService projectNetworkService,
            IInputService inputService,
            IObjectsInRoomService objectsInRoomService
        )
        {
            _matchInfoController = matchInfoController;
            _projectNetworkService = projectNetworkService;
            _inputService = inputService;
            _objectsInRoomService = objectsInRoomService;
        }

        public void Enter()
        {
            _inputService.Enable();
            foreach (var playerView in _objectsInRoomService.PlayerViews)
                playerView.SubscribeOnInput();
            _matchInfoController.Show();
        }

        public void Exit()
        {
            _projectNetworkService.IsGameStarted = false;
        }
    }
}