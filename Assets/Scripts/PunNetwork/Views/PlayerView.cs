using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services;
using UnityEngine;
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

        [SerializeField] private float _rotationSpeed = 90.0f;
        [SerializeField] private MeshRenderer _teamMarker;
        [SerializeField] private float _movementSpeed = 2.0f;
        [SerializeField] private float _maxSpeed = 0.2f;
        [SerializeField] private ParticleSystem _destruction;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Image[] _heartImages;

        public PhotonView PhotonView { get; private set; }
        public bool IsSpawnedOnServer { get; set; }

        private Rigidbody _rigidbody;
        private Collider _collider;
        private float _rotation;
        private float _acceleration;
        private float _shootingTimer;
        private bool _controllable = true;
        private MeshRenderer[] _renderers;

        #region UNITY

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }

        public void Start()
        {
            foreach (var r in GetComponentsInChildren<Renderer>())
            {
                r.material.color = AsteroidsGame.GetColor(PhotonView.Owner.GetPlayerNumber());
            }
        }

        public void Update()
        {
            if (!PhotonView.AmOwner || !_controllable || !IsSpawnedOnServer)
                return;

            // we don't want the master client to apply input to remote ships while the remote player is inactive
            if (PhotonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
                return;

            _rotation = Input.GetAxis("Horizontal");
            _acceleration = Input.GetAxis("Vertical");

            if (Input.GetButton("Jump") && _shootingTimer <= 0.0)
            {
                _shootingTimer = 0.2f;

                PhotonView.RPC(nameof(Fire), RpcTarget.AllViaServer, _rigidbody.position, _rigidbody.rotation);
            }

            if (_shootingTimer > 0.0f)
                _shootingTimer -= Time.deltaTime;
        }

        public void FixedUpdate()
        {
            if (!PhotonView.IsMine || !_controllable || !IsSpawnedOnServer)
                return;

            var rot = _rigidbody.rotation * Quaternion.Euler(0, _rotation * _rotationSpeed * Time.fixedDeltaTime, 0);
            _rigidbody.MoveRotation(rot);

            var force = rot * Vector3.forward * _acceleration * 1000.0f * _movementSpeed * Time.fixedDeltaTime;
            _rigidbody.AddForce(force);

            if (_rigidbody.velocity.magnitude > _maxSpeed * 1000.0f)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed * 1000.0f;

            //CheckExitScreen();
        }

        #endregion

        #region PUN CALLBACKS

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (info.Sender.IsLocal)
                info.Sender.SetCustomProperties(new Hashtable
                    { { PlayerProperty.IsSpawned.ToString(), true } });
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
            
            /*PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            PhotonNetwork.LeaveRoom();*/
        }

        [PunRPC]
        public void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            var lag = (float)(PhotonNetwork.Time - info.SentServerTime);

            var bullet =
                /** Use this if you want to fire one bullet at a time **/
                Instantiate(_bulletPrefab, position, Quaternion.identity);
            bullet.GetComponent<Bullet>()
                .InitializeBullet(rotation * Vector3.forward, Mathf.Abs(lag));


            /** Use this if you want to fire two bullets at once **/
            //Vector3 baseX = rotation * Vector3.right;
            //Vector3 baseZ = rotation * Vector3.forward;

            //Vector3 offsetLeft = -1.5f * baseX - 0.5f * baseZ;
            //Vector3 offsetRight = 1.5f * baseX - 0.5f * baseZ;

            //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetLeft, Quaternion.identity) as GameObject;
            //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
            //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetRight, Quaternion.identity) as GameObject;
            //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
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