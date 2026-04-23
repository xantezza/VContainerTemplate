using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace VContainerTemplate.Infrastructure.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Inject] private IAudioProvider _audioProvider;
        
        private readonly Dictionary<SFXClip, AudioSource> _sfxSources = new();
        private readonly Dictionary<MusicClip, AudioSource> _musicSources = new();
        
        public void PlaySFX(SFXClip sfxClip, bool restartIfAlreadyExists = true)
        {
            if (_sfxSources.TryGetValue(sfxClip, out var existSource) && restartIfAlreadyExists)
            {
                existSource.Stop();
                existSource.Play();
                return;
            }

            var source = gameObject.AddComponent<AudioSource>();
            var container = _audioProvider.SFXClips[sfxClip];
            source.clip = container.audioClip;
            source.volume = container.volume01;
            if (container.randomPitch) source.pitch = Random.Range(container.pitchFrom, container.pitchTo);
            source.outputAudioMixerGroup = _audioProvider.SFXGroup;
            source.Play();
            _sfxSources.Add(sfxClip, source);
        }

        public void PlayMusic(MusicClip musicClip, bool restartIfAlreadyExists = true)
        {
            if (_musicSources.TryGetValue(musicClip, out var existSource))
            {
                if (!restartIfAlreadyExists) return;
                existSource.Stop();
                existSource.Play();
                return;
            }

            var source = gameObject.AddComponent<AudioSource>();
            var container = _audioProvider.MusicClips[musicClip];
            source.clip = container.audioClip;
            source.volume = container.volume01;
            if (container.randomPitch) source.pitch = Random.Range(container.pitchFrom, container.pitchTo);
            source.outputAudioMixerGroup = _audioProvider.MusicGroup;
            source.Play();
            _musicSources.Add(musicClip, source);
        }

        public void StopMusic(MusicClip musicClip)
        {
            if (_musicSources.TryGetValue(musicClip, out var existSource))
            {
                existSource.Stop();
            }
        }

        public void StopMusic()
        {
            foreach (var musicSourcesValue in _musicSources.Values)
            {
                musicSourcesValue.Stop();
            }
        }
    }
}