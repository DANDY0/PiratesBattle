using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Utils;
using Zenject;
using Logger = Utils.Logger;

namespace PunNetwork.Views
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Renderer))]
    public class Bullet : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
    {
        public bool IsActive { get; private set; }

        private Rigidbody _rb;
        private IBulletsPool _pool;
        private float _bulletSpeed = 10f;
        private int _ownerID;
        private bool _isDestroyed;

        [Inject] private IBulletsPool _bulletsPool;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Init(int ownerID)
        {
            _ownerID = ownerID;
            photonView.TransferOwnership(_ownerID);
            
            gameObject.SetActive(false);
            _isDestroyed = false;
        }

        private void SetUp(Vector3 position, Quaternion rotation, float lag)
        {
            transform.position = position;
            transform.rotation = rotation;

            Vector3 correctedPosition = position + (transform.forward * _bulletSpeed * lag);
            transform.position = correctedPosition;
    
            Logger.Log($"lag: {lag}, bulletPos: {transform.position}, bullet rot: {transform.rotation}," +
                       $" correctedPos: {correctedPosition}", nameof(SetUp));
        }

        [PunRPC]
        private void RPCFire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            Init(info.Sender.ActorNumber);
            SetUp(position, rotation, Mathf.Abs(lag));
            SetActive(true);
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPCFire), RpcTarget.AllViaServer, position, rotation);
        }

        [PunRPC]
        private void RPCDeactivate()
        {
            Logger.Log($"bulletPos: {transform.position}, bullet rot: {transform.rotation}",nameof(RPCDeactivate));
            SetActive(false);
            
            _rb.velocity = Vector3.zero;
            _rb.position = Vector3.zero;
            _rb.rotation = Quaternion.identity;
        }

        private void Deactivate()
        {
            photonView.RPC(nameof(RPCDeactivate), RpcTarget.All);
        }

        private void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            IsActive = isActive;
        }

        private void FixedUpdate()
        {
            if (!IsActive)
                return;

            _rb.velocity = transform.forward * _bulletSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isDestroyed || !photonView.IsMine)
                return;

            var playerView = other.GetComponent<PlayerView>();

            if (playerView != null && playerView.TeamRole == Enumerators.TeamRole.EnemyPlayer)
            {
                other.GetComponent<PhotonView>().RPC(nameof(PlayerView.RegisterHit), RpcTarget.AllViaServer);
                photonView.Owner.AddScore(1);
                Debug.Log(
                    $"Player{photonView.Owner.ActorNumber} damaged Player{playerView.PhotonView.Owner.ActorNumber}");
                Deactivate();
                // ReturnToPool();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Deactivate();
                // ReturnToPool();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_ownerID);
            }
            else
            {
                _ownerID = (int)stream.ReceiveNext();
            }
        }

        /*private void ReturnToPool()
        {
            _pool.ReturnBullet(this);
        }*/

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            _pool = DependencyInjector.Container.Resolve<IBulletsPool>();
            _pool.Init(this);
        }
    }
}