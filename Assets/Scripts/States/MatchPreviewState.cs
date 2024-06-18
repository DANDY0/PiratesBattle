using Controllers;
using PunNetwork.Services.ObjectsInRoom;
using Services.Input;
using States.Core;

namespace States
{
    public class MatchPreviewState : IState
    {
        private readonly PreviewMatchAnimationController _previewMatchAnimationController;
        private readonly IInputService _inputService;
        private readonly IObjectsInRoomService _objectsInRoomService;

        public MatchPreviewState
        (
            PreviewMatchAnimationController previewMatchAnimationController,
            IInputService inputService,
            IObjectsInRoomService objectsInRoomService
        )
        {
            _previewMatchAnimationController = previewMatchAnimationController;
            _inputService = inputService;
            _objectsInRoomService = objectsInRoomService;
        }

        public void Enter()
        {
            _previewMatchAnimationController.Start();
        }

        public void Exit()
        {
            _inputService.Enable();
            foreach (var playerView in _objectsInRoomService.PlayerViews) 
                playerView.SubscribeOnInput();
            _previewMatchAnimationController.Hide();
        }
    }
}