using PunNetwork.Views;
using UnityEngine.Serialization;

namespace PunNetwork
{
    using UnityEngine;
    using Photon.Pun;
    using System.Collections.Generic;

    public class BulletsPool : MonoBehaviourPunCallbacks, IBulletsPool
    {
        public GameObject _bulletPrefab;
        public int _initialPoolSize = 20;

        private List<Bullet> _bulletPool;
        private int _poolIndex = 0;

        void Start()
        {
            _bulletPool = new List<Bullet>();
            for (int i = 0; i < _initialPoolSize; i++)
            {
                GameObject bulletObj = PhotonNetwork.InstantiateRoomObject(_bulletPrefab.name, Vector3.zero, Quaternion.identity);
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bulletObj.SetActive(false);
                _bulletPool.Add(bullet);
            }
        }

        public Bullet GetBullet()
        {
            for (int i = 0; i < _bulletPool.Count; i++)
            {
                _poolIndex = (_poolIndex + 1) % _bulletPool.Count;
                if (!_bulletPool[_poolIndex].gameObject.activeInHierarchy)
                {
                    return _bulletPool[_poolIndex];
                }
            }

            GameObject newBulletObj = PhotonNetwork.InstantiateRoomObject(_bulletPrefab.name, Vector3.zero, Quaternion.identity);
            Bullet newBullet = newBulletObj.GetComponent<Bullet>();
            newBulletObj.SetActive(false);
            _bulletPool.Add(newBullet);
            return newBullet;
        }

        public void SpawnBullet(Vector3 position, Quaternion rotation, int ownerID)
        {
            Bullet bullet = GetBullet();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.Init(ownerID, this);
            bullet.gameObject.SetActive(true);
        }

        public void ReturnBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }
    }

    public interface IBulletsPool
    {
        public Bullet GetBullet();
        public void SpawnBullet(Vector3 position, Quaternion rotation, int ownerID);
        public void ReturnBullet(Bullet bullet);
    }
}