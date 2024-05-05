using System;
using Enums;
using UnityEngine.Audio;

namespace Models
{
    [Serializable]
    public class AudioMixerGroupVo
    {
        public EVolumeType Type;
        public AudioMixerGroup Group;
    }
}