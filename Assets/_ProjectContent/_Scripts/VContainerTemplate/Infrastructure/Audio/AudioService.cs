using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainerTemplate.Infrastructure.Logger;

namespace VContainerTemplate.Infrastructure.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Inject] private IAudioProvider _audioProvider;

        private readonly Dictionary<SFXClip, AudioEntry> _sfxEntries = new();
        private readonly Dictionary<MusicClip, AudioEntry> _musicEntries = new();
        private readonly List<AudioSource> _overlapSources = new();
        private readonly Stack<AudioSource> _availableSources = new();

        public void PlaySFX(SFXClip sfxClip, bool restartIfAlreadyExists = true)
        {
            _ = PlaySFXAsync(sfxClip, restartIfAlreadyExists);
        }

        public void PlayMusic(MusicClip musicClip, bool restartIfAlreadyExists = true)
        {
            _ = PlayMusicAsync(musicClip, restartIfAlreadyExists);
        }

        public void StopMusic(MusicClip musicClip)
        {
            if (_musicEntries.TryGetValue(musicClip, out var entry))
            {
                entry.Source?.Stop();
            }
        }

        public void StopMusic()
        {
            foreach (var entry in _musicEntries.Values)
            {
                entry.Source?.Stop();
            }
        }

        private async Task PlaySFXAsync(SFXClip sfxClip, bool restartIfAlreadyExists)
        {
            if (_sfxEntries.TryGetValue(sfxClip, out var existEntry))
            {
                if (!await WaitForLoad(existEntry)) return;

                if (restartIfAlreadyExists)
                {
                    existEntry.Source.Stop();
                    existEntry.Source.Play();
                    return;
                }

                PlayOverlap(existEntry.Clip, _audioProvider.SFXClips[sfxClip], _audioProvider.SFXGroup);
                return;
            }

            var container = _audioProvider.SFXClips[sfxClip];
            existEntry = new AudioEntry();
            _sfxEntries.Add(sfxClip, existEntry);
            await LoadEntry(existEntry, container, _audioProvider.SFXGroup);
            if (!existEntry.IsLoaded) _sfxEntries.Remove(sfxClip);
        }

        private async Task PlayMusicAsync(MusicClip musicClip, bool restartIfAlreadyExists)
        {
            if (_musicEntries.TryGetValue(musicClip, out var existEntry))
            {
                if (!await WaitForLoad(existEntry)) return;

                if (!restartIfAlreadyExists) return;

                existEntry.Source.Stop();
                existEntry.Source.Play();
                return;
            }

            var container = _audioProvider.MusicClips[musicClip];
            existEntry = new AudioEntry();
            _musicEntries.Add(musicClip, existEntry);
            await LoadEntry(existEntry, container, _audioProvider.MusicGroup);
            if (!existEntry.IsLoaded) _musicEntries.Remove(musicClip);
        }

        private async Task<bool> WaitForLoad(AudioEntry entry)
        {
            if (entry.IsLoaded) return true;

            return await entry.LoadCompletion.Task;
        }

        private async Task LoadEntry(AudioEntry entry, AudioDataContainer container, AudioMixerGroup outputGroup)
        {
            var handle = container.audioClip.LoadAssetAsync();

            try
            {
                await handle.Task;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load audio clip from Addressables: {container.audioClip.RuntimeKey}\n{e}", LogTag.Audio);
                Addressables.Release(handle);
                entry.LoadCompletion.TrySetResult(false);
                return;
            }

            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                Log.Error($"Failed to load audio clip from Addressables: {container.audioClip.RuntimeKey}", LogTag.Audio);
                Addressables.Release(handle);
                entry.LoadCompletion.TrySetResult(false);
                return;
            }

            var source = PlaySource(handle.Result, container, outputGroup);
            entry.Source = source;
            entry.Clip = handle.Result;
            entry.Handle = handle;
            entry.IsLoaded = true;
            entry.LoadCompletion.TrySetResult(true);
        }

        private void PlayOverlap(AudioClip clip, AudioDataContainer container, AudioMixerGroup outputGroup)
        {
            var source = PlaySource(clip, container, outputGroup);
            _overlapSources.Add(source);
            RecycleWhenFinished(source).Forget();
        }

        private async UniTask RecycleWhenFinished(AudioSource source)
        {
            while (source != null && source.isPlaying)
            {
                await UniTask.Yield();
            }

            if (source == null) return;

            source.Stop();
            source.clip = null;
            _overlapSources.Remove(source);
            _availableSources.Push(source);
        }

        private AudioSource PlaySource(AudioClip clip, AudioDataContainer container, AudioMixerGroup outputGroup)
        {
            var source = GetSource();
            source.clip = clip;
            source.volume = container.volume01;
            if (container.randomPitch) source.pitch = UnityEngine.Random.Range(container.pitchFrom, container.pitchTo);
            source.outputAudioMixerGroup = outputGroup;
            source.Play();
            return source;
        }

        private AudioSource GetSource()
        {
            if (_availableSources.Count > 0) return _availableSources.Pop();
            return gameObject.AddComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            foreach (var entry in _sfxEntries.Values) entry.Release();
            foreach (var entry in _musicEntries.Values) entry.Release();

            foreach (var source in _overlapSources)
            {
                if (source != null) Destroy(source);
            }
            _overlapSources.Clear();

            foreach (var source in _availableSources)
            {
                if (source != null) Destroy(source);
            }
            _availableSources.Clear();
        }

        private sealed class AudioEntry
        {
            public AudioSource Source;
            public AudioClip Clip;
            public AsyncOperationHandle<AudioClip> Handle;
            public TaskCompletionSource<bool> LoadCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            public bool IsLoaded;

            public void Release()
            {
                if (Source != null)
                {
                    Source.Stop();
                    Destroy(Source);
                }

                if (Handle.IsValid())
                {
                    Addressables.Release(Handle);
                }
            }
        }
    }
}