﻿using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.Demo.Asteroids
{
    public class BulletAsteroids : MonoBehaviour
    {
        public Player Owner { get; private set; }

        public void Start()
        {
            Destroy(gameObject, 3.0f);
        }

        public void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }

        public void InitializeBullet(Player owner, Vector3 originalDirection, float lag)
        {
            Owner = owner;

            transform.forward = originalDirection;

            var rb = GetComponent<Rigidbody>();
            rb.velocity = originalDirection * 200.0f;
            rb.position += rb.velocity * lag;
        }
    }
}