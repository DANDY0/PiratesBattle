using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Utils; 

namespace PunNetwork.Views
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Renderer))]

    public class Bullet : MonoBehaviour, IPunObservable
    {
        private PhotonView _photonView;
        private Rigidbody _rb;
        private IBulletsPool _pool;
    
        private float _bulletSpeed = 10f;
        private int _ownerID;
        private bool _isDestroyed;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _rb = GetComponent<Rigidbody>();
        }

        public void Init(int ownerID, IBulletsPool pool)
        {
            this._ownerID = ownerID;
            _pool = pool;
            _isDestroyed = false;
        }

        private void FixedUpdate()
        {
            if (!_photonView.IsMine)
                return;
        
            _rb.velocity = transform.forward * _bulletSpeed;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (_isDestroyed || !_photonView.IsMine)
                return;
        
            var playerView = collider.GetComponent<PlayerView>();

            if (_photonView.IsMine && playerView != null && playerView.TeamRole == Enumerators.TeamRole.EnemyPlayer)
            {
                collider.GetComponent<PhotonView>().RPC(nameof(PlayerView.RegisterHit), RpcTarget.All);
                _photonView.Owner.AddScore(1);
                Debug.LogError($"Player{_photonView.Owner.ActorNumber} damaged Player{playerView.PhotonView.Owner.ActorNumber}");
                ReturnToPool();
            }
            if (_photonView.IsMine && collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                ReturnToPool();
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

        private void ReturnToPool()
        {
            _isDestroyed = true;
            gameObject.SetActive(false);
            _pool.ReturnBullet(this);
        }
    }
}