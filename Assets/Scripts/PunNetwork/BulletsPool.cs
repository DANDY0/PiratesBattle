using System.Linq;
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

        private List<Bullet> _bullets;
        private int _initialPoolSize = 20;
        private int _poolIndex = 0;
        private string _bulletPath = "TeamPlayers/ExBullet";

        private Transform _parent;

        public void Initialize()
        {
            _parent = new GameObject(nameof(BulletsPool)).transform;

            _bullets = new List<Bullet>();
            if (!PhotonNetwork.IsMasterClient)
                return;

            for (var i = 0; i < _initialPoolSize; i++) 
                CreateBullet();
        }

        public Bullet SpawnBullet(Vector3 position, Quaternion rotation)
        {
            var bullet = GetBullet();
            
            bullet.Fire(position, rotation);

            return bullet;
        }

        private Bullet GetBullet()
        {
            foreach (var bullet in _bullets)
                if(!bullet.IsActive)
                    return bullet;

            return CreateBullet();
        }

        private Bullet CreateBullet()
        {
            var bulletObj = PhotonNetwork.InstantiateRoomObject(_bulletPath, Vector3.zero,
                Quaternion.identity);

            var bullet = bulletObj.GetComponent<Bullet>();
            Init(bullet);
        
            return bullet;
        }

        public void Init(Bullet bullet)
        {
            bullet.transform.SetParent(_parent);
            bullet.Init(PhotonNetwork.LocalPlayer.ActorNumber);
            _bullets.Add(bullet);
        }
    }

    public interface IBulletsPool
    {
        public Bullet SpawnBullet(Vector3 position, Quaternion rotation);
        void Init(Bullet bullet);
    }
}