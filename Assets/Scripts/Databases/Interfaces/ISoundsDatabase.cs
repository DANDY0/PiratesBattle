using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Databases.Interfaces
{
    public interface ISoundsDatabase
    {
        IEnumerable<ESoundtrackName> SoundtrackTypes { get; }
        
        AudioClip GetSfxClip(ESoundFxName soundName);
        AudioClip GetSoundtracksClip(ESoundtrackName type);
    }
}