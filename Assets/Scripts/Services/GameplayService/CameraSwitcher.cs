using System;
using GameplayEntities;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Views.Player;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.GameplayService
{
    public class CameraSwitcher: MonoBehaviour, ICameraSwitcher
    {
        [SerializeField] private GameplayCamera _overviewCamera;
        [SerializeField] private GameplayCamera _playerCamera;
        
        private GameplayCamera _activeCamera;
        private IRoomPlayersService _roomPlayersService;
        private PlayerView _playerView;

        [Inject]
        public void Construct(IRoomPlayersService roomPlayersService)
        {
            _roomPlayersService = roomPlayersService;
        }

        public void Init()
        {
            _playerView = _roomPlayersService.LocalView;
        }
        
        public void SwitchToPlayerCamera()
        {
            SetActiveCamera(Enumerators.CameraType.PlayerCamera);
            _activeCamera.SetFollowTarget(_playerView.transform);
        }

        public void SwitchToOverviewCamera()
        {
            SetActiveCamera(Enumerators.CameraType.OverView);
            _activeCamera.SetFollowTarget(null);
        }

        private void SetActiveCamera(Enumerators.CameraType cameraType)
        {
            if(_activeCamera != null)
                _activeCamera.SetPriority(0);
    
            _activeCamera = GetCameraByType(cameraType);
            
            _activeCamera.SetPriority(1);
        }

        private GameplayCamera GetCameraByType(Enumerators.CameraType cameraType)
        {
            switch (cameraType)
            {
                case Enumerators.CameraType.PlayerCamera:
                    return _playerCamera;
                case Enumerators.CameraType.OverView:
                    return _overviewCamera;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null);
            }
        }
    }
}