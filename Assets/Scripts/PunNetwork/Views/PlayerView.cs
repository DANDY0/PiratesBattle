using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services;
using Services.Data;
using Services.Input;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using Zenject;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using static Utils.Enumerators;

namespace PunNetwork.Views
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
        
        private IInputService _inputService;
        private IDataService _dataService;
        private IBulletsPool _bulletsPool;

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

        #region UNITY

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _characterController = GetComponent<CharacterController>();

            _bulletsPool = DependencyInjector.Container.Resolve<IBulletsPool>();
            _inputService = DependencyInjector.Container.Resolve<IInputService>();
            _dataService = DependencyInjector.Container.Resolve<IDataService>();

            Debug.Log("InputService" + nameof(_inputService));

        }

        void Update()
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
            Vector3 move = new Vector3(_inputService.MoveAxis.x, 0, _inputService.MoveAxis.y);
            if (move.sqrMagnitude > 1)
                move.Normalize();

            move *= _speed;
            _characterController.Move(move * Time.deltaTime);

            if (_inputService.IsPreciseFiring)
                RotateTowards(new Vector3(_inputService.LookAxis.x, 0, _inputService.LookAxis.y));
            else
                RotateTowards(move);
        }

        private void HandleShooting()
        {
            if ((_inputService.IsFiring) && _shootingTimer <= 0.0f)
            {
                _shootingTimer = 0.2f;  // Reset shooting cooldown
                _bulletsPool.SpawnBullet(transform.position, transform.rotation);
            }

            if (_shootingTimer > 0.0f)
                _shootingTimer -= Time.deltaTime;
        }

        private void RotateTowards(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);
            }
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