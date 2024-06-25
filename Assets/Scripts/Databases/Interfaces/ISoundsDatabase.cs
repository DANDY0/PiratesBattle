using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Audio;

namespace Databases
{
    public interface ISoundsDatabase
    {
        IEnumerable<ESoundtrackName> SoundtrackTypes { get; }
        
        AudioClip GetSfxClip(ESoundFxName soundName);
        AudioClip GetSoundtracksClip(ESoundtrackName type);
    }
}