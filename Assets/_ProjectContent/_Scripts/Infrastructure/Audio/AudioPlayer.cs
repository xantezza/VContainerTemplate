using TriInspector;
using UnityEngine;
using VContainer;

namespace VContainerTemplate.Infrastructure.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [Inject] private IAudioService _audioService;
        
        [SerializeField] private bool _playOnEnable;
        [SerializeField] private bool _stopOnDisable;
        [SerializeField] private bool _sfx;
        [SerializeField] [ShowIf("_sfx")] private SFXClip _sfxClip;
        [SerializeField] [HideIf("_sfx")] private MusicClip _musicClip;
        
        private void OnEnable()
        {
            if (_playOnEnable)
            {
                if (_sfx) _audioService.PlaySFX(_sfxClip);
                else _audioService.PlayMusic(_musicClip);
            }
        }

        public void Play()
        {
            if (_sfx) _audioService.PlaySFX(_sfxClip);
            else _audioService.PlayMusic(_musicClip);
        }

        public void PlaySFX(SFXClip clip)
        {
            _audioService.PlaySFX(clip);
        }
        
        public void PlayMusic(MusicClip clip)
        {
            _audioService.PlayMusic(clip);
        }

        private void OnDisable()
        {
            if (_stopOnDisable && !_sfx) _audioService.StopMusic(_musicClip);
        }
    }
}