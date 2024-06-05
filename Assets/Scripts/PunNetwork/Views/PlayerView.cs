using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services;
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

        [SerializeField] private float _rotationSpeed = 10.0f;
        [SerializeField] private MeshRenderer _teamMarker;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private ParticleSystem _destruction;
        [SerializeField] private Image[] _heartImages;

        public PhotonView PhotonView { get; private set; }
        public bool IsSpawnedOnServer { get; set; }

        private Rigidbody _rigidbody;
        private Collider _collider;
        private CharacterController _characterController;
        private float _rotation;
        private float _acceleration;
        private float _shootingTimer;
        private bool _controllable = true;
        private MeshRenderer[] _renderers;

        private IBulletsPool _bulletsPool;
        
        #region UNITY

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _characterController = GetComponent<CharacterController>();
            
            _bulletsPool = DependencyInjector.Container.Resolve<IBulletsPool>();
        }
        
        void Update()
        {
            if (!PhotonView.AmOwner || !_controllable || !IsSpawnedOnServer)
                return;

            if (PhotonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
                return;

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (move.magnitude > 1)
                move.Normalize();

            move *= _speed;

            _characterController.Move(move * Time.deltaTime);

            if (move != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * _rotationSpeed);
            }

            if ((Input.GetButton("Jump") || Input.GetMouseButtonDown(1)) && _shootingTimer <= 0.0f)
            {
                _shootingTimer = 0.2f;

                if (PhotonView.IsMine) 
                    _bulletsPool.SpawnBullet(transform.position, transform.rotation);
                
            }

            if (_shootingTimer > 0.0f)
                _shootingTimer -= Time.deltaTime;
        }        

        #endregion

        #region PUN CALLBACKS

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (info.Sender.IsLocal)
                info.Sender.SetCustomProperty(PlayerProperty.IsSpawned, true);
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