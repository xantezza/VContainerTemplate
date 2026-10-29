namespace VContainerTemplate.Infrastructure.Audio
{
    public interface IAudioService
    {
        void PlaySFX(SFXClip sfxClip, bool restartIfAlreadyExists = true);
        void PlayMusic(MusicClip musicClip, bool restartIfAlreadyExists = true);
        void StopMusic(MusicClip musicClip);
    }
}