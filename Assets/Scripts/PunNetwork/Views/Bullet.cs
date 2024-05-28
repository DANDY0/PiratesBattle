    using System;
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
        private Rigidbody _rb;
        private float _bulletSpeed = 10f;

        #region UNITY

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            // DOVirtual.DelayedCall(3, DestroyBullet);
        }

        private void FixedUpdate()
        {
            // transform.Translate(Vector3.forward * 0.1f);
            _rb.velocity = transform.forward * _bulletSpeed;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (_isDestroyed)
                return;

            var playerView = collider.GetComponent<PlayerView>();

            if (_photonView.IsMine && playerView != null && playerView.TeamRole == Enumerators.TeamRole.EnemyPlayer)
            {
                collider.GetComponent<PhotonView>().RPC(nameof(PlayerView.RegisterHit), RpcTarget.All);
                _photonView.Owner.AddScore(1);
                Debug.LogError($"Player{_photonView.Owner.ActorNumber} damaged Player{playerView.PhotonView.Owner.ActorNumber}");
                DestroyBullet();
            }
            if (_photonView.IsMine && collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                DestroyBullet();
            
        }
        #endregion

        public void InitializeBullet(Vector3 originalDirection)
        {
            /*transform.forward = originalDirection;

            var rb = GetComponent<Rigidbody>();
            rb.velocity = originalDirection * _bulletSpeed;
            rb.position += rb.velocity;
            
            Debug.Log("pos:" + rb.transform.position);*/
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