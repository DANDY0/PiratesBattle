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
    public class Bullet : PhotonPoolObjPunCallbacks, IPunObservable
    {
        private Rigidbody _rb;
        private const float BulletSpeed = 10f;
        private int _ownerID;
        private bool _isDestroyed;
        private bool _isActive;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void SetOwner(int ownerID)
        {
            _ownerID = ownerID;
            photonView.TransferOwnership(_ownerID);

            _isDestroyed = false;
        }

        private void Activate(Vector3 position, Quaternion rotation, float lag)
        {
            var correctedPosition = position + transform.forward * BulletSpeed * lag;

            //_poolService.ActivatePoolItem<Bullet>(GameObjectEntryKey.Bullet.ToString(), correctedPosition, rotation);

            Logger.Log($"lag: {lag}, bulletPos: {transform.position}, bullet rot: {transform.rotation}," +
                       $" correctedPos: {correctedPosition}", nameof(Activate));
        }

        [PunRPC]
        private void RPCFire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            var lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            SetOwner(info.Sender.ActorNumber);
            Activate(position, rotation, Mathf.Abs(lag));
            _isActive = true;
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPCFire), RpcTarget.AllViaServer, position, rotation);
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
            PhotonNetwork.Destroy(gameObject);
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
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Deactivate();
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

        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            base.OnPhotonInstantiate(info);
            SetOwner(info.Sender.ActorNumber);
        }
    }
}