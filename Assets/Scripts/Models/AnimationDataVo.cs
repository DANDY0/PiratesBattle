using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Models
{
    [Serializable]
    public class AnimationDataVo
    {
        public List<AnimationData> AnimationsData;
    }
    
    [Serializable]
    public class AnimationData
    {
        public Enumerators.RigAnimatorState RigState;
        public Vector3 WeaponPosition;
        public Vector3 WeaponRotation;
        public Vector3 RightRefPosition;
        public Vector3 RightRefRotation;
        public Vector3 RightHintPosition;
        public Vector3 LeftRefPosition;
        public Vector3 LeftRefRotation;
        public Vector3 LeftHintPosition;
    }
}