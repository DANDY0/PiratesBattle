using System;
using Cinemachine;
using UnityEngine;
using Utils;

namespace GameplayEntities
{
    [Serializable]
    public class GameplayCamera
    {
        [SerializeField] private CinemachineVirtualCamera Camera;
        // public Enumerators.CameraType CameraType { get; set; }
        
        public void SetFollowTarget(Transform target)
        {
            Camera.Follow = target;
        }
        
        public void SetPriority(int priority)
        {
            Camera.Priority = priority;
        }
    }
}