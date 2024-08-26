using Photon.PhotonUnityNetworking.Code.Common;
using Services.Input;
using Services.PhotonPool;
using UnityEngine;

namespace PunNetwork.Views.Player
{
    public class PlayerShooting
    {
        private PlayerView _playerView;
        private IInputService _inputService;
        private IPhotonPoolService _photonPoolService;
        private EnemiesTriggerCollider _enemiesTriggerCollider;
        private PlayerAnimator _playerAnimator;
        private float _initialShootingDelay;
        private float _shootingTimer;
        private bool _isFiring;

        public PlayerShooting(PlayerView playerView, IInputService inputService, IPhotonPoolService photonPoolService,
            EnemiesTriggerCollider enemiesTriggerCollider, PlayerAnimator playerAnimator)
        {
            _playerView = playerView;
            _inputService = inputService;
            _photonPoolService = photonPoolService;
            _enemiesTriggerCollider = enemiesTriggerCollider;
            _playerAnimator = playerAnimator;
        }

        public void Initialize()
        {
            // Any initialization logic if needed
        }

        public void FireJoystickTriggered(bool state)
        {
            if (state)
            {
                StartFiring();
                _playerAnimator.FireAim(true);
            }
            else
            {
                _isFiring = false;
                _playerAnimator.FireAim(false);
            }
            
        }

        private void StartFiring()
        {
            _isFiring = true;
            _initialShootingDelay = 0.25f;
        }

        public void HandleShooting()
        {
            if (!_isFiring)
                return;

            if (_initialShootingDelay > 0)
            {
                _initialShootingDelay -= Time.deltaTime;
                if (_initialShootingDelay <= 0)
                    _shootingTimer = 0;
                return;
            }

            if (_shootingTimer <= 0)
            {
                _shootingTimer = .2f;

                var position = _playerView.transform.position;
                var rotation = _playerView.transform.rotation;

                var bullet = _photonPoolService.ActivatePoolItem<Bullet.Bullet>(
                    Enumerators.GameObjectEntryKey.Bullet.ToString(),
                    position,
                    rotation);
                bullet.Fire(position);
                
            }

            if (_shootingTimer > 0)
                _shootingTimer -= Time.deltaTime;
        }
    }
}