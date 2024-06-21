using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.ObjectPooling;
using PunNetwork.Views.Player;
using Services.GamePools;
using UnityEngine;
using Utils;
using Zenject;
using Logger = Utils.Logger;
using static Utils.Enumerators;

namespace PunNetwork.Views.Bullet
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Renderer))]
    public class Bullet : PhotonPoolObject, IPunObservable
    {
        private Rigidbody _rb;
        private const float BulletSpeed = 10f;
        private bool _isActive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPCFire), RpcTarget.AllViaServer, position, rotation);
        }

        [PunRPC]
        private void RPCFire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            var lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.position = position + transform.forward * BulletSpeed * lag;

            _isActive = true;
        }
        
        [PunRPC]
        private void RPCDeactivate()
        {
            Logger.Log($"bulletPos: {transform.position}, bullet rot: {transform.rotation}", nameof(RPCDeactivate));
            _isActive = false;
            //_poolService.DisablePoolItem(GameObjectEntryKey.Bullet.ToString(), this);
        }

        private void Deactivate()
        {
            PhotonPoolService.DisablePoolItem(GameObjectEntryKey.Bullet.ToString(), this);
            photonView.RPC(nameof(RPCDeactivate), RpcTarget.All);
        }
        
        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            _rb.velocity = transform.forward * BulletSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive || !photonView.IsMine)
                return;

            var playerView = other.GetComponent<PlayerView>();

            if (playerView != null && playerView.TeamRole == TeamRole.EnemyPlayer)
            {
                playerView.PhotonView.RPC(nameof(PlayerView.RegisterHit), RpcTarget.AllViaServer);
                photonView.Owner.AddScore(1);
                Debug.Log(
                    $"Player{photonView.Owner.ActorNumber} damaged Player{playerView.PhotonView.Owner.ActorNumber}");
                Deactivate();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Deactivate();
            }
        }
    }
}