using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Audio;
using VContainerTemplate.Utilities.ScriptableObjects;

namespace VContainerTemplate.Infrastructure.Audio
{
    public class AudioProvider : ACScriptableObject, IAudioProvider
    {
        [field: SerializeField] public AudioMixerGroup SFXGroup { get; private set; }
        [field: SerializeField] public AudioMixerGroup MusicGroup { get; private set; }
        
        [SerializeField] private SerializedDictionary<SFXClip, AudioDataContainer> _SFXClips;
        [SerializeField] private SerializedDictionary<MusicClip, AudioDataContainer> _musicClips;

        public IReadOnlyDictionary<SFXClip, AudioDataContainer> SFXClips => _SFXClips;
        public IReadOnlyDictionary<MusicClip, AudioDataContainer> MusicClips => _musicClips;
    }
}