using System.Collections.Generic;
using UnityEngine.Audio;

namespace VContainerTemplate.Infrastructure.Audio
{
    public interface IAudioProvider
    {
        AudioMixerGroup SFXGroup { get; }
        AudioMixerGroup MusicGroup { get; }
        IReadOnlyDictionary<SFXClip, AudioDataContainer> SFXClips { get; }
        IReadOnlyDictionary<MusicClip, AudioDataContainer> MusicClips { get; }
    }
}