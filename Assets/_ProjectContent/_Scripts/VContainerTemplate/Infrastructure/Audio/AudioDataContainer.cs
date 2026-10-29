using System;
using TriInspector;
using UnityEngine;

namespace VContainerTemplate.Infrastructure.Audio
{
    [Serializable]
    public class AudioDataContainer
    {
        public AudioClipReference audioClip;
        [Range(0, 1)] public float volume01 = 1f;
        public bool randomPitch = false;
        [ShowIf(nameof(randomPitch))] public float pitchFrom = 1f;
        [ShowIf(nameof(randomPitch))] public float pitchTo = 1f;
    }
}