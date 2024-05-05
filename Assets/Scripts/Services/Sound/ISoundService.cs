using Enums;

namespace Services.Sound
{
    public interface ISoundService
    {
        void PlayMusicSingle(ESoundtrackName soundtrack);
        void PlaySound(ESoundFxName clip);
        void PlayMusic(ESoundtrackName musicType, bool isQueue = false, bool loop = true);
        void StopAll();
        void PlayMusicQueue(bool isLoop = true);
        void PlayRandomSound(params ESoundFxName[] clipNames);
    }
}