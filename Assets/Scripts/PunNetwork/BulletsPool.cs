using PunNetwork.Views;
using UnityEngine.Serialization;
using Zenject;

namespace PunNetwork
{
    using UnityEngine;
    using Photon.Pun;
    using System.Collections.Generic;

    public class BulletsPool : IInitializable, IBulletsPool
    {
        public GameObject _bulletPrefab;
        public int _initialPoolSize = 20;

        private Queue<Bullet> _bulletQueue;
        private int _poolIndex = 0;
        private string _bulletPath = "TeamPlayers/Bullet";

        private Transform _parent;

        public void Initialize()
        {
            _parent = new GameObject(nameof(BulletsPool)).transform;

            _bulletQueue = new Queue<Bullet>();
            if (!PhotonNetwork.IsMasterClient)
                return;

            for (var i = 0; i < _initialPoolSize; i++) 
                CreateBullet();
        }

        public Bullet SpawnBullet(Vector3 position, Quaternion rotation, int ownerID)
        {
            var bullet = GetBullet();
            
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.Init(ownerID);
            bullet.SetActive(true);
            
            return bullet;
        }

        public void ReturnBullet(Bullet bullet)
        {
            bullet.SetActive(false);
            _bulletQueue.Enqueue(bullet);
        }

        public void Setup(Bullet bullet)
        {
            bullet.transform.SetParent(_parent);
            bullet.SetActive(false);
            _bulletQueue.Enqueue(bullet);
        }

        private Bullet CreateBullet()
        {
            var bulletObj = PhotonNetwork.InstantiateRoomObject(_bulletPath, Vector3.zero, Quaternion.identity);

            var bullet = bulletObj.GetComponent<Bullet>();
            Setup(bullet);
            
            return bullet;
        }

        private Bullet GetBullet() => _bulletQueue.TryDequeue(out var bullet) ? bullet : CreateBullet();
    }

    public interface IBulletsPool
    {
        public Bullet SpawnBullet(Vector3 position, Quaternion rotation, int ownerID);
        public void ReturnBullet(Bullet bullet);
        void Setup(Bullet bullet);
    }
}