using DG.Tweening;
using Photon.Pun;
using Services.Input;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;
using static Utils.Enumerators;

namespace PunNetwork.Views.Player
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback
    {
        public TeamRole TeamRole { get; private set; }
        public PhotonView PhotonView { get; private set; }
        public bool IsSpawnedOnServer { get; set; }

        [SerializeField] private MeshRenderer _teamMarker;
        [SerializeField] private ParticleSystem _destruction;
        [SerializeField] private Image[] _heartImages;
		[SerializeField] private PlayerUI _playerUI;
		[SerializeField] private EnemiesTriggerCollider _enemiesTriggerCollider;
        private IInputService _inputService;

        private Rigidbody _rigidbody;
        private Collider _collider;
        private CharacterController _characterController;
        private MeshRenderer[] _renderers;

        private float _rotationSpeed = 150f;
        private float _speed = 3f;
        private float _rotation;
        private float _acceleration;
        private float _shootingTimer;
        private bool _controllable = true;
        private Tween _animationTween;
        private bool _isFiring;

        [Inject]
        private void Construct
        (
            IInputService inputService
        )
        {
            _inputService = inputService;
        }

        public void SubscribeOnInput()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent += FireJoystickTriggered;
        }
        
        #region UNITY

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _characterController = GetComponent<CharacterController>();
            
            Debug.Log("InputService" + nameof(_inputService));
        }

        private void OnDestroy()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent -= FireJoystickTriggered;
        }

        private void FireJoystickTriggered(bool state)
        {
            if (state)
                StartFiring();
            else
            {
                _isFiring = false;
                if (_animationTween == null) return;
                _animationTween.Kill();
                _animationTween = null;
            }
        }

        private void StartFiring()
        {
            _isFiring = true;
            _animationTween = DOVirtual.DelayedCall(.25f, () => _shootingTimer = 0);
        }

        private void Update()
        {
            if (!CanControl())
                return;

            HandleMovement();
            HandleShooting();
        }

        private bool CanControl()
        {
            return PhotonView.IsMine && gameObject.activeInHierarchy;
        }

        private void HandleMovement()
        {
            var move = new Vector3(_inputService.MoveAxis.x, 0, _inputService.MoveAxis.y);
            if (move.sqrMagnitude > 1)
                move.Normalize();

            move *= _speed;
            _characterController.Move(move * Time.deltaTime);

            HandleRotation(move);
        }

        private void HandleRotation(Vector3 move)
        {
            const float lookAxisThreshold = .1f;

            if (_isFiring)
            {
                if (_inputService.LookAxis.sqrMagnitude < lookAxisThreshold * lookAxisThreshold)
                {
                    if (_enemiesTriggerCollider.TryGetNearestEnemy(out var enemy)) 
                        RotateTowards(enemy.position - transform.position);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);
        }


        private void HandleShooting()
        {
            if (!_isFiring)
                return;
            if (_shootingTimer <= 0)
            {
                _shootingTimer = .2f;
                var bullet = PhotonNetwork.InstantiatePoolRoomObject(GameObjectEntryKey.Bullet.ToString(), transform.position,
                    transform.rotation);
                bullet.GetComponent<Bullet.Bullet>().Fire(transform.position, transform.rotation);
            }

            if (_shootingTimer > 0)
                _shootingTimer -= Time.deltaTime;
        }

        #endregion

        #region PUN CALLBACKS

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (info.Sender.IsLocal)
            {
                info.Sender.SetCustomProperty(PlayerProperty.IsSpawned, true);
            }
        }

        [PunRPC]
        public void RegisterHit()
        {
            if (!PhotonView.IsMine ||
                !PhotonNetwork.LocalPlayer.TryGetCustomProperty(PlayerProperty.PlayerLives, out var lives)) return;
            var currentLives = (int)lives <= 1 ? 0 : (int)lives - 1;
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.PlayerLives, currentLives);

            if (currentLives == 0)
                PhotonView.RPC(nameof(DestroyPlayer), RpcTarget.All);
        }

        [PunRPC]
        public void DestroyPlayer()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _collider.enabled = false;
            foreach (var meshRenderer in _renderers)
                meshRenderer.enabled = false;

            _controllable = false;

            _destruction.Play();
        }

        #endregion

        public void SetTeamRole(TeamRole role)
        {
            TeamRole = role;
            Debug.Log($"{role}");
            var markerColor = TeamMarker.GetColor(role);
            _teamMarker.material.color = markerColor;
        }

        public void SetNickname(string nickname) 
            => _playerUI.SetNickName(nickname);

        public void UpdateHearts(int currentLives)
        {
            for (var i = 0; i < _heartImages.Length; i++)
                _heartImages[i].enabled = i < currentLives;
        }
    }

    public static class TeamMarker
    {
        public static Color GetColor(TeamRole role) =>
            role switch
            {
                TeamRole.MyPlayer => Color.green,
                TeamRole.AllyPlayer => Color.blue,
                TeamRole.EnemyPlayer => Color.red,
            };
    }
}