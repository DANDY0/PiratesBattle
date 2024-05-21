    using DG.Tweening;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using Utils;

namespace PunNetwork.Views
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Renderer))]
    public class Bullet : MonoBehaviour
    {
        private bool _isDestroyed;

        private PhotonView _photonView;

        #region UNITY

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void Start() => DOVirtual.DelayedCall(3, DestroyBullet);

        private void OnCollisionEnter(Collision collision)
        {
            if (_isDestroyed)
                return;

            var o = collision.gameObject;
            var playerView = o.GetComponent<PlayerView>();
    
            if (_photonView.IsMine && playerView != null && playerView.TeamRole == Enumerators.TeamRole.EnemyPlayer)
            {
                o.GetComponent<PhotonView>().RPC(nameof(PlayerView.RegisterHit), RpcTarget.All);
                _photonView.Owner.AddScore(1);
                DestroyBullet();
            }
        }
        #endregion

        public void InitializeBullet(Vector3 originalDirection, float lag)
        {
            transform.forward = originalDirection;

            var rb = GetComponent<Rigidbody>();
            rb.velocity = originalDirection * 200.0f;
            rb.position += rb.velocity * lag;
        }

        private void DestroyBullet()
        {
            _isDestroyed = true;
            
            GetComponent<Renderer>().enabled = false;
            
            if (_photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }
}