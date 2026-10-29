using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VContainerTemplate.Infrastructure.Audio
{
    [Serializable]
    public class AudioClipReference : AssetReferenceT<AudioClip>
    {
        public AudioClipReference(string guid) : base(guid) { }
    }
}