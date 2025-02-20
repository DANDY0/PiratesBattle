﻿using Databases;
using Services.Input;
using UnityEngine;

namespace PunNetwork.Views.Player
{
    public class PlayerMovement
    {
        private PlayerView _playerView;
        private PlayerAnimator _playerAnimator;
        private IInputService _inputService;
        private readonly PlayerShooting _playerShooting;
        private CharacterController _characterController;
        private EnemiesTriggerCollider _enemiesTriggerCollider;

        private Rigidbody _rigidbody;
        private float _rotationSpeed = 12f;
        private float _speed = 3f;

        public PlayerMovement(PlayerView playerView, IInputService inputService, PlayerShooting playerShooting,
            EnemiesTriggerCollider enemiesTriggerCollider,
            PlayerAnimator playerAnimator,
            IAnimationConfigurationsDatabase animationConfigurationsDatabase)
        {
            _playerView = playerView;
            _inputService = inputService;
            _playerShooting = playerShooting;
            _enemiesTriggerCollider = enemiesTriggerCollider;

            _playerAnimator = playerAnimator;
            _playerAnimator.Initialize(animationConfigurationsDatabase);
        }

        public void Initialize()
        {
            _rigidbody = _playerView.GetComponent<Rigidbody>();
            _characterController = _playerView.GetComponent<CharacterController>();
        }

        public bool CanControl()
        {
            return _playerView.PhotonView.IsMine && _playerView.gameObject.activeInHierarchy;
        }

        public void HandleMovement()
        {
            var inputVector = new Vector3(_inputService.MoveAxis.x, 0, _inputService.MoveAxis.y);
            var move = inputVector;
            if (move.sqrMagnitude > 1)
                move.Normalize();

            move *= _speed;
            _characterController.Move(move * Time.deltaTime);

            HandleRotation(move);
            
            _playerAnimator.MoveAnimation(inputVector.magnitude > 0);
        }
        
        private void HandleRotation(Vector3 move)
        {
            const float lookAxisThreshold = .1f;

            if (_playerShooting.IsFiring)
            {
                if (_inputService.LookAxis.sqrMagnitude < lookAxisThreshold * lookAxisThreshold)
                {
                    if (_enemiesTriggerCollider.TryGetNearestEnemy(out var enemy)) 
                        RotateTowards(enemy.position - _playerView.transform.position);
                }
                else
                    RotateTowards(new Vector3(_inputService.LookAxis.x, 0, _inputService.LookAxis.y));
            }
            else
                RotateTowards(move);
        }

        private void RotateTowards(Vector3 direction)
        {
            direction.y = 0;

            if (direction == Vector3.zero) return;
            var newRotation = Quaternion.LookRotation(direction);
            _playerView.transform.rotation = Quaternion.Slerp(_playerView.transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);
        }
    }
}