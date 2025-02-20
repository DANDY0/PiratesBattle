﻿using UnityEngine;

namespace Photon.PhotonUnityNetworking.Code.Common.Factory
{
    public interface IGameFactory
    {
        GameObject CreateFromResources(string key, Vector3 zero, Quaternion identity);
        T Create<T>(T prefab, Vector3 posToSpawn, Quaternion rotation) where T : Component;
        GameObject Create(GameObject prefab, Vector3 posToSpawn, Quaternion rotation);
        GameObject Create(GameObject prefab, Vector3 posToSpawn, Quaternion rotation, Transform parent);
        GameObject CreateWithKey(string key, Vector3 posToSpawn, Quaternion rotation);
    }
}